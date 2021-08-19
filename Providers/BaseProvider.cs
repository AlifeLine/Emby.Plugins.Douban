using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Emby.Plugins.Douban.Response;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Logging;
using System.Text.RegularExpressions;

namespace Emby.Plugins.Douban
{
    public abstract class BaseProvider
    {
        /// <summary>
        /// Used to store douban Id in Jellyfin system. 
        /// </summary>
        public const string ProviderID = "DoubanID";

        protected readonly ILogger _logger;

        protected readonly Configuration.PluginConfiguration _config;

        // All requests 
        protected readonly IDoubanClient _doubanClient;

        protected BaseProvider(IJsonSerializer jsonSerializer, ILogger logger)
        {
            this._logger = logger;
            this._config = Plugin.Instance == null ?
                               new Configuration.PluginConfiguration() :
                               Plugin.Instance.Configuration;

            this._doubanClient = new FrodoAndroidClient(jsonSerializer, logger);
        }

        public Task<HttpResponseMessage> GetImageResponse(string url,
           CancellationToken cancellationToken)
        {
            _logger.LogCallerInfo("[DOUBAN] GetImageResponse url: {0}", url);
            return _doubanClient.GetAsync(url, cancellationToken);
        }

        public async Task<List<Response.SearchTarget>> Search<T>(string name,
            CancellationToken cancellationToken)
        {
            MediaType type = typeof(T) == typeof(Movie) ? MediaType.movie : MediaType.tv;

            _logger.LogCallerInfo($"[DOUBAN] Searching for sid of {type} named \"{name}\"");

            var searchResults = new List<Response.SearchTarget>();

            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogCallerInfo($"[DOUBAN] Search name is empty.");
                return searchResults;
            }

            name = name.Replace('.', ' ');

            try
            {
                var response = await _doubanClient.Search(name, cancellationToken);
                if (response.Items.Count > 0)
                {
                    searchResults = response.Items.Where(item => item.Target_Type == type.ToString())
                        .Select(item => item.Target).ToList();

                    if (searchResults.Count == 0)
                    {
                        _logger.LogCallerInfo($"[DOUBAN] Seems like \"{name}\" genre is not {type}.");
                    }
                }
                else
                {
                    _logger.LogCallerInfo($"[DOUBAN] No results found for \"{name}\".");
                }
            }
            catch (HttpRequestException)
            {
                //_logger.LogError($"[DOUBAN] Search \"{name}\" error, got {e.StatusCode}.");
                throw;
            }

            _logger.LogCallerInfo($"[DOUBAN] Finish searching {name}, count: {searchResults.Count}");
            return searchResults;
        }

        protected async Task<Response.Subject> GetSubject<T>(string sid,
            CancellationToken cancellationToken) where T : BaseItem
        {
            MediaType type = typeof(T) == typeof(Movie) ? MediaType.movie : MediaType.tv;
            return await _doubanClient.GetSubject(sid, type, cancellationToken);
        }

        protected async Task<MetadataResult<T>> GetMetadata<T>(string sid, CancellationToken cancellationToken)
        where T : BaseItem, new()
        {
            _logger.Info("GetMetadata T");
            var result = new MetadataResult<T>();

            MediaType type = typeof(T) == typeof(Movie) ? MediaType.movie : MediaType.tv;
            var subject = await _doubanClient.GetSubject(sid, type, cancellationToken);
            var subjectCredits=await _doubanClient.GetSubjectCredits(sid, type, cancellationToken);
            result.Item = TransMediaInfo<T>(subject);
            result.Item.SetProviderId(ProviderID, sid);
            TransPersonInfo(subjectCredits.credits.ElementAt(0).celebrities, PersonType.Director).ForEach(result.AddPerson);
            TransPersonInfo(subjectCredits.credits.ElementAt(1).celebrities, PersonType.Actor).ForEach(result.AddPerson);
            result.QueriedById = true;
            result.HasMetadata = true;

            return result;
        }

        private static T TransMediaInfo<T>(Subject data) where T : BaseItem, new()
        {
            var item = new T
            {
                Name = data.Title ?? data.Original_Title,
                OriginalTitle = data.Original_Title,
                CommunityRating = data.Rating?.Value,
                Overview = data.Intro,
                ProductionYear = int.Parse(data.Year),
                //HomePageUrl = data.Url,
                ProductionLocations = data.Countries?.ToArray()
            };

            if (data.Pubdate?.Count > 0 && !String.IsNullOrEmpty(data.Pubdate[0]))
            {
                //string pubdate = data.Pubdate[0].Split('(', 2)[0];
                string pubdate = data.Pubdate[0].Split('(')[0];
                if (DateTime.TryParse(pubdate, out DateTime dateValue))
                {
                    item.PremiereDate = dateValue;
                }
            }

            if (data.Trailer != null)
            {
                item.AddTrailerUrl(data.Trailer.Video_Url);
            }

            data.Genres.ForEach(item.AddGenre);

            return item;
        }

        private static List<PersonInfo> TransPersonInfo(
            List<CelebritiesItem> CelebritiesList, PersonType personType)
        {
            var result = new List<PersonInfo>();
            foreach (var Celebrities in CelebritiesList)
            {
                var role = Regex.Replace(Celebrities.character, @"(.*\()(.*)(\).*)", "$2");
                var personInfo = new PersonInfo
                {
                    Name = Celebrities.name,
                    Type = personType,  
                    ImageUrl = Celebrities.avatar.large ?? "",
                    Role = role?? Celebrities.character
                };

                personInfo.SetProviderId(ProviderID, Celebrities.id);
                result.Add(personInfo);
            }
            return result;
        }
    }
}