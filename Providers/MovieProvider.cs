using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using MediaBrowser.Model.Logging;
using Emby.Plugins.JavScraper.Services;
using System;

namespace Emby.Plugins.Douban.Providers
{
    public class MovieProvider : BaseProvider, IHasOrder,
        IRemoteMetadataProvider<Movie, MovieInfo>
    {
        public string Name => "Douban Emby Movie Provider";
        public int Order => 3;
        private readonly IHttpClient _httpClient;
        private readonly ImageProxyService imageProxyService;
        public MovieProvider(IHttpClient http, IJsonSerializer jsonSerializer,
            ILogger logger) : base(jsonSerializer, logger)
        {
            _httpClient = http;
            // Empty
        }

        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info,
            CancellationToken cancellationToken)
        {
            _logger.LogCallerInfo($"[DOUBAN] Getting metadata for \"{info.Name}\"");

            string sid = info.GetProviderId(ProviderID);
            if (string.IsNullOrWhiteSpace(sid))
            {
                var searchResults = await Search<Movie>(info.Name, cancellationToken);
                sid = searchResults.FirstOrDefault()?.Id;
            }

            if (string.IsNullOrWhiteSpace(sid))
            {
                _logger.LogCallerInfo($"[DOUBAN] No sid found for \"{info.Name}\"");
                return new MetadataResult<Movie>();
            }

            var result = await GetMetadata<Movie>(sid, cancellationToken);
            if (result.HasMetadata)
            {
                _logger.LogCallerInfo($"[DOUBAN] Get the metadata of \"{info.Name}\" successfully!");
                info.SetProviderId(ProviderID, sid);
            }

            return result;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(
            MovieInfo info, CancellationToken cancellationToken)
        {
            _logger.LogCallerInfo($"[DOUBAN] GetSearchResults \"{info.Name}\"");

            var results = new List<RemoteSearchResult>();

            var searchResults = new List<Response.SearchTarget>();

            string sid = info.GetProviderId(ProviderID);
            if (!string.IsNullOrEmpty(sid))
            {
                var subject = await GetSubject<Movie>(sid, cancellationToken);
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
                searchResults = await Search<Movie>(info.Name, cancellationToken);
            }

            foreach (Response.SearchTarget searchTarget in searchResults)
            {
                _logger.LogCallerInfo("Cover_Url"+searchTarget?.Cover_Url);

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
        private string GetLocalUrl(string url, ImageType type = ImageType.Backdrop)
        {
            if (string.IsNullOrEmpty(url))
                return url;
            if (url.IndexOf("Plugins/alifeline_douban/Image", StringComparison.OrdinalIgnoreCase) >= 0)
                return url;
            return $"/emby/Plugins/alifeline_douban/Image?url={url}&type={type}";
        }
        Task<HttpResponseInfo> IRemoteSearchProvider.GetImageResponse(string url, CancellationToken cancellationToken)
        {
            _logger.Info("Mv GetImageResponse url:" + url);
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
    }
}