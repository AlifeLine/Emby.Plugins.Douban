using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System;

namespace Emby.Plugins.Douban.Providers
{
    public class TVProvider : BaseProvider, IHasOrder,
           IRemoteMetadataProvider<Series, SeriesInfo>,
           IRemoteMetadataProvider<Season, SeasonInfo>,
           IRemoteMetadataProvider<Episode, EpisodeInfo>
    {
        public string Name => "Douban Emby TV Provider";
        public int Order => 3;
        private readonly IHttpClient _httpClient;
        public TVProvider(IHttpClient http, IJsonSerializer jsonSerializer,
                          ILogger logger) : base(jsonSerializer, logger)
        {
            _httpClient = http;
            // empty
        }

        #region series
        public async Task<MetadataResult<Series>> GetMetadata(SeriesInfo info,
            CancellationToken cancellationToken)
        {
            _logger.LogCallerInfo($"[DOUBAN] Getting series metadata for \"{info.Name}\"");

            var sid = info.GetProviderId(ProviderID);
            if (string.IsNullOrWhiteSpace(sid))
            {
                var searchResults = await Search<Series>(info.Name, cancellationToken);
                sid = searchResults.FirstOrDefault()?.Id;
            }

            if (string.IsNullOrWhiteSpace(sid))
            {
                _logger.LogCallerInfo($"[DOUBAN] No sid found for \"{info.Name}\"");
                return new MetadataResult<Series>();
            }

            var result = await GetMetadata<Series>(sid, cancellationToken);
            if (result.HasMetadata)
            {
                info.SetProviderId(ProviderID, sid);
                result.QueriedById = true;
                _logger.LogCallerInfo($"[DOUBAN] Get series metadata of \"{info.Name}\" successfully!");
            }

            return result;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(
            SeriesInfo info, CancellationToken cancellationToken)
        {
            _logger.LogCallerInfo($"[DOUBAN] Searching series \"{info.Name}\"");

            var results = new List<RemoteSearchResult>();

            var searchResults = new List<Response.SearchTarget>();

            string sid = info.GetProviderId(ProviderID);
            if (!string.IsNullOrEmpty(sid))
            {
                var subject = await GetSubject<Series>(sid, cancellationToken);
                searchResults.Add(new Response.SearchTarget
                {
                    Id = subject?.Id,
                    Cover_Url = subject?.Pic?.Normal,
                    Year = subject?.Year,
                    Title = subject?.Title
                });
            }
            else
            {
                searchResults = await Search<Series>(info.Name, cancellationToken);
            }

            foreach (Response.SearchTarget searchTarget in searchResults)
            {
                _logger.LogCallerInfo("Cover_Url" + searchTarget?.Cover_Url);
                var searchResult = new RemoteSearchResult()
                {
                    Name = searchTarget?.Title,
                    ImageUrl = GetLocalUrl(searchTarget?.Cover_Url, ImageType.Thumb),
                    ProductionYear = int.Parse(searchTarget?.Year)
                };
                searchResult.SetProviderId(ProviderID, searchTarget.Id);
                results.Add(searchResult);
            }

            return results;
        }
        #endregion series

        #region season
        public async Task<MetadataResult<Season>> GetMetadata(SeasonInfo info,
            CancellationToken cancellationToken)
        {
            _logger.LogCallerInfo($"[DOUBAN] Getting season metadata for \"{info.Name}\"");
            var result = new MetadataResult<Season>();

            info.SeriesProviderIds.TryGetValue(ProviderID, out string sid);
            if (string.IsNullOrEmpty(sid))
            {
                _logger.LogCallerInfo("No douban sid found, just skip");
                return result;
            }

            if (string.IsNullOrWhiteSpace(sid))
            {
                _logger.LogCallerInfo($"[DOUBAN FRODO ERROR] No sid found for \"{info.Name}\"");
                return new MetadataResult<Season>();
            }

            var subject = await GetSubject<Season>(sid, cancellationToken).
                ConfigureAwait(false);

            string pattern_name = @".* (?i)Season(?-i) (\d+)$";
            Match match = Regex.Match(subject.Original_Title, pattern_name);
            if (match.Success)
            {
                result.Item = new Season
                {
                    IndexNumber = int.Parse(match.Groups[1].Value),
                    ProductionYear = int.Parse(subject.Year)
                };
                result.HasMetadata = true;
            }
            return result;

        }

        public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(
            SeasonInfo info, CancellationToken cancellationToken)
        {
            _logger.LogCallerInfo("Douban:Search for season {0}", info.Name);
            // It's needless for season to do search
            return Task.FromResult<IEnumerable<RemoteSearchResult>>(
                new List<RemoteSearchResult>());
        }
        #endregion season

        #region episode
        public async Task<MetadataResult<Episode>> GetMetadata(EpisodeInfo info,
                                              CancellationToken cancellationToken)
        {
            _logger.LogCallerInfo($"Douban:GetMetadata for episode {info.Name}");
            var result = new MetadataResult<Episode>();

            if (info.IsMissingEpisode)
            {
                _logger.LogCallerInfo("Do not support MissingEpisode");
                return result;
            }

            var sid = info.GetProviderId(ProviderID);
            if (string.IsNullOrEmpty(sid))
            {
                var searchResults = await Search<Episode>(info.Name, cancellationToken);
                sid = searchResults.FirstOrDefault()?.Id;
            }

            if (!info.IndexNumber.HasValue)
            {
                _logger.LogCallerInfo("No episode num found, please check " +
                    "the format of file name");
                return result;
            }
            // Start to get information from douban
            result.Item = new Episode
            {
                Name = info.Name,
                IndexNumber = info.IndexNumber,
                ParentIndexNumber = info.ParentIndexNumber
            };
            result.Item.SetProviderId(ProviderID, sid);

            //var url = string.Format("https://movie.douban.com/subject/{0}" +
            //    "/episode/{1}/", sid, info.IndexNumber);
            // TODO(Libitum)
            // string content = await _doubanAccessor.GetResponseWithDelay(url, cancellationToken);
            string content = "";
            string pattern_name = "data-name=\\\"(.*?)\\\"";
            Match match = Regex.Match(content, pattern_name);
            if (match.Success)
            {
                var name = HttpUtility.HtmlDecode(match.Groups[1].Value);
                _logger.LogCallerInfo("The name is {0}", name);
                result.Item.Name = name;
            }

            string pattern_desc = "data-desc=\\\"(.*?)\\\"";
            match = Regex.Match(content, pattern_desc);
            if (match.Success)
            {
                var desc = HttpUtility.HtmlDecode(match.Groups[1].Value);
                _logger.LogCallerInfo("The desc is {0}", desc);
                result.Item.Overview = desc;
            }
            result.HasMetadata = true;

            return result;
        }
        private string GetLocalUrl(string url, ImageType type = ImageType.Backdrop)
        {
            if (string.IsNullOrEmpty(url))
                return url;
            if (url.IndexOf("Plugins/alifeline_douban/Image", StringComparison.OrdinalIgnoreCase) >= 0)
                return url;
            return $"/emby/Plugins/alifeline_douban/Image?url={url}&type={type}";
        }

        public Task<IEnumerable<RemoteSearchResult>> GetSearchResults(
            EpisodeInfo info, CancellationToken cancellationToken)
        {
            _logger.LogCallerInfo("Douban:Search for episode {0}", info.Name);
            // It's needless for season to do search
            return Task.FromResult<IEnumerable<RemoteSearchResult>>(
                new List<RemoteSearchResult>());
        }
        Task<HttpResponseInfo> IRemoteSearchProvider.GetImageResponse(string url, CancellationToken cancellationToken)
        {
            _logger.Info("TV GetImageResponse url:" + url);
            if (url.IndexOf("Plugins/alifeline_douban/Image", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                url = url.Replace("/emby/Plugins/alifeline_douban/Image?url=", "");
            }
            var res = _httpClient.GetResponse(new HttpRequestOptions
            {
                Url = url,
                CancellationToken = cancellationToken
            });
            return res;
        }
        #endregion episode
    }
}
