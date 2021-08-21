using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Extensions;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Net;
using System.Text.RegularExpressions;

namespace Emby.Plugins.Douban.Providers
{
    public class doubanPersonProvider : BaseProvider, IRemoteMetadataProvider<Person, PersonLookupInfo>
    {
        const string DataFileName = "info.json";
        internal static doubanPersonProvider Current { get; private set; }

        private readonly IJsonSerializer _jsonSerializer;
        private readonly IFileSystem _fileSystem;
        private readonly IServerConfigurationManager _configurationManager;
        private readonly IHttpClient _httpClient;
        public doubanPersonProvider(IFileSystem fileSystem, IServerConfigurationManager configurationManager, IJsonSerializer jsonSerializer, IHttpClient httpClient, ILogger logger) : base(jsonSerializer, logger)
        {
            _fileSystem = fileSystem;
            _configurationManager = configurationManager;
            _jsonSerializer = jsonSerializer;
            _httpClient = httpClient;
            Current = this;
        }

        public string Name
        {
            get { return "DouBan"; }
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(PersonLookupInfo searchInfo, CancellationToken cancellationToken)
        {
            var douabnId = searchInfo.GetProviderId(ProviderID);
            _logger.Info("douabnId" + douabnId);
            if (!string.IsNullOrEmpty(douabnId))
            {
                await EnsurePersonInfo(douabnId, cancellationToken).ConfigureAwait(false);

                var dataFilePath = GetPersonDataFilePath(_configurationManager.ApplicationPaths, douabnId);
                _logger.Info("dataFilePath" + dataFilePath);
                var info = _jsonSerializer.DeserializeFromFile<Response.ElessarSubject>(dataFilePath);
                var result = new RemoteSearchResult
                {
                    Name = info.title,

                    SearchProviderName = Name,

                    ImageUrl = info.cover.large.url ?? ""
                };

                result.SetProviderId(ProviderID, info.id);
                foreach (var data in info.extra.info)
                {
                    if (data[0] == "IMDb编号")
                    {
                        result.SetProviderId(MetadataProviders.Imdb, data[1]);
                    }
                }
                return new[] { result };
            }
            return new List<RemoteSearchResult>();
        }

        public async Task<MetadataResult<Person>> GetMetadata(PersonLookupInfo id, CancellationToken cancellationToken)
        {
            var doubanId = id.GetProviderId(ProviderID);
            _logger.Info(doubanId);
            // We don't already have an Id, need to fetch it
            if (string.IsNullOrEmpty(doubanId))
            {
                doubanId = await GetDoubanId(id, cancellationToken).ConfigureAwait(false);
            }
            _logger.Info(doubanId);
            var result = new MetadataResult<Person>();

            if (!string.IsNullOrEmpty(doubanId))
            {
                await EnsurePersonInfo(doubanId, cancellationToken).ConfigureAwait(false);
                var dataFilePath = GetPersonDataFilePath(_configurationManager.ApplicationPaths, doubanId);

                var info = _jsonSerializer.DeserializeFromFile<Response.ElessarSubject>(dataFilePath);

                var item = new Person();
                result.HasMetadata = true;

                // Take name from incoming info, don't rename the person
                // TODO: This should go in PersonMetadataService, not each person provider
                item.Name = id.Name;
                DateTime date;
                //item.HomePageUrl = info.homepage;
                foreach (var data in info.extra.info)
                {
                    if(data[0]== "出生地")
                    {
                        item.ProductionLocations = new string[] { data[1] };
                    }
                    else if (data[0] == "IMDb编号")
                    {
                        item.SetProviderId(MetadataProviders.Imdb, data[1]);
                    }
                    else if (data[0] == "出生日期")
                    {
                        if (DateTime.TryParse(data[1].Replace("+",""), out date))
                        {
                            item.PremiereDate = date.ToUniversalTime();
                        }
                    }
  
                }
                var desc= Regex.Matches(info.desc, @"<p>.*</p>")[0].Value;
                desc = desc.Replace("<p>", "");
                desc = desc.Replace("</p>", "");
                item.Overview = desc;
                item.SetProviderId(ProviderID, info.id);

                result.HasMetadata = true;
                result.Item = item;
            }

            return result;
        }

        private readonly CultureInfo cuture = new CultureInfo("zh-cn");

        /// <summary>
        /// Gets the TMDB id.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task{System.String}.</returns>
        private async Task<string> GetDoubanId(PersonLookupInfo info, CancellationToken cancellationToken)
        {
            var results = await GetSearchResults(info, cancellationToken).ConfigureAwait(false);

            return results.Select(i => i.GetProviderId(ProviderID)).FirstOrDefault();
        }

        internal async Task EnsurePersonInfo(string doubanId, CancellationToken cancellationToken)
        {
            _logger.Info("EnsurePersonInfo" + doubanId);
            var dataFilePath = GetPersonDataFilePath(_configurationManager.ApplicationPaths, doubanId);

            var fileInfo = _fileSystem.GetFileSystemInfo(dataFilePath);

            if (fileInfo.Exists && (DateTime.UtcNow - _fileSystem.GetLastWriteTimeUtc(fileInfo)).TotalDays <= 2)
            {
                return;
            }


            var result = await getPersonInfo(doubanId, cancellationToken);
            _fileSystem.CreateDirectory(_fileSystem.GetDirectoryName(dataFilePath));
            System.IO.File.WriteAllText(dataFilePath,_jsonSerializer.SerializeToString(result));
        }
        private static string GetPersonDataPath(IApplicationPaths appPaths, string doubanId)
        {

            return Path.Combine(GetPersonsDataPath(appPaths), doubanId);
        }

        internal static string GetPersonDataFilePath(IApplicationPaths appPaths, string doubanId)
        {
            return Path.Combine(GetPersonDataPath(appPaths, doubanId), DataFileName);
        }

        private static string GetPersonsDataPath(IApplicationPaths appPaths)
        {
            return Path.Combine(appPaths.CachePath, "douban-people");
        }
        public Task<HttpResponseInfo> GetImageResponse(string url, CancellationToken cancellationToken)
        {
            return _httpClient.GetResponse(new HttpRequestOptions
            {
                CancellationToken = cancellationToken,
                Url = url
            });
        }
    }
}