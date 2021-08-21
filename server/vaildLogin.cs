using System.Threading.Tasks;
using MediaBrowser.Common.Extensions;
using MediaBrowser.Controller.Net;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Services;
using MediaBrowser.Model.Logging;
using Emby.Plugins.JavScraper.Services;

namespace Emby.Plugins.Douban.Services
{
    /// <summary>
    /// 转发图片信息
    /// </summary>
    [Route("/emby/Plugins/alifeline_douban/login", "GET")]
    public class loginInfo  
    {
        /// <summary>
        /// 图像类型
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string password { get; set; }
    }

    public class loginServer : IService, IRequiresRequest   
    {
        private readonly ImageProxyService imageProxyService;
        private readonly IHttpResultFactory resultFactory;
        private readonly ILogger logger;

        /// <summary>
        /// Gets or sets the request context.
        /// </summary>
        /// <value>The request context.</value>
        public IRequest Request { get; set; }

        public loginServer(
            ILogManager logManager,
            ImageProxyService imageProxyService,
            IHttpResultFactory resultFactory
                           )
        {
            this.imageProxyService = imageProxyService;
            this.resultFactory = resultFactory;
        }

        public object Get(loginInfo request)
            => DoGet(request?.username, request?.password);

        /// <summary>
        /// 转发信息
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task<object> DoGet(string username, string? password)
        {
            //logger.Info($"{url}");

            //if (url.IsWebUrl() != true)
            //   throw new ResourceNotFoundException();

            //var resp =
            //if (!(resp?.ContentLength > 0))
            //    throw new ResourceNotFoundException();

            //return resultFactory.GetResult(Request, resp.Content, resp.ContentType);
            return null;
        }
    }
}