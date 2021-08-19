using Emby.Plugins.Douban.server;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
namespace Emby.Plugins.JavScraper.Services
{
    /// <summary>
    /// 图片代理服务
    /// </summary>
    public class ImageProxyService
    {
        private HttpClientEx client;
        private static FileExtensionContentTypeProvider fileExtensionContentTypeProvider = new FileExtensionContentTypeProvider();

        public ImageProxyService(IServerApplicationHost serverApplicationHost, IJsonSerializer jsonSerializer, ILogger logger, IFileSystem fileSystem, IApplicationPaths appPaths)
        {
            client = new HttpClientEx();
            this.serverApplicationHost = serverApplicationHost;
            this.jsonSerializer = jsonSerializer;
            this.logger = logger;
            this.fileSystem = fileSystem;
            this.appPaths = appPaths;
        }
        private readonly IServerApplicationHost serverApplicationHost;
        private readonly IJsonSerializer jsonSerializer;
        private readonly ILogger logger;
        private readonly IFileSystem fileSystem;
        private readonly IApplicationPaths appPaths;

        public async Task<HttpResponseInfo> GetImageResponse(string url, ImageType type, CancellationToken cancellationToken)
        {
            logger?.Info($"{nameof(GetImageResponse)}-{url}");

            var key = WebUtility.UrlEncode(url);
            var cache_file = Path.Combine(appPaths.GetImageCachePath().ToString(), key);
            byte[] bytes = null;

            //尝试从缓存中读取
            try
            {
                var fi = fileSystem.GetFileInfo(cache_file);

                //图片文件存在，且是24小时之内的
                if (fi.Exists && fileSystem.GetFileInfo(cache_file).LastWriteTimeUtc > DateTime.Now.AddDays(-1).ToUniversalTime())
                {
                    bytes = await fileSystem.ReadAllBytesAsync(cache_file);
                    logger?.Info($"Hit image cache {url} {cache_file}");

                    fileExtensionContentTypeProvider.TryGetContentType(url, out var contentType);

                    return new HttpResponseInfo()
                    {
                        Content = new MemoryStream(bytes),
                        ContentLength = bytes.Length,
                        ContentType = contentType ?? "image/jpeg",
                        StatusCode = HttpStatusCode.OK,
                    };
                }
            }
            catch (Exception ex)
            {
                logger?.Warn($"Read image cache error. {url} {cache_file} {ex.Message}");
            }

            try
            {
                var resp = await client.GetAsync(url, cancellationToken);
                if (resp.IsSuccessStatusCode == false)
                    return await Parse(resp);

                try
                {
                    fileSystem.WriteAllBytes(cache_file, await resp.Content.ReadAsByteArrayAsync());
                    logger?.Info($"Save image cache {url} {cache_file} ");
                }
                catch (Exception ex)
                {
                    logger?.Warn($"Save image cache error. {url} {cache_file} {ex.Message}");
                }
                return await Parse(resp);
            }
            catch (Exception ex)
            {
                logger?.Error(ex.ToString());
            }
            return new HttpResponseInfo();
        }
        /// <summary>
        /// 构造本地url地址
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetLocalUrl(string url, ImageType type = ImageType.Backdrop)
        {
            logger?.Info("start get local url");
            if (string.IsNullOrEmpty(url))
                return url;
            if (url.IndexOf("Plugins/alifeline_douban/Image", StringComparison.OrdinalIgnoreCase) >= 0)
                return url;
            return $"/emby/Plugins/alifeline_douban/Image?url={HttpUtility.UrlEncode(url)}&type={type}";
        }
        private async Task<HttpResponseInfo> Parse(HttpResponseMessage resp)
        {
            var r = new HttpResponseInfo()
            {
                Content = await resp.Content.ReadAsStreamAsync(),
                ContentLength = resp.Content.Headers.ContentLength,
                ContentType = resp.Content.Headers.ContentType?.ToString(),
                StatusCode = resp.StatusCode,
                Headers =
                resp.Content.Headers.ToDictionary(o => o.Key, o => string.Join(", ", o.Value))
            };
            return r;
        }
    }
}