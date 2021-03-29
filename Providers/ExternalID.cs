using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Audio;

namespace Emby.Plugins.Douban
{
    public class DoubanExternalId : IExternalId
    {
        public string ProviderName => "Douban";

        public string Key => BaseProvider.ProviderID;

        public string UrlFormatString => "https://movie.douban.com/subject/{0}/";

        public string Name
        {
            get { return "Douban"; }
        }

        public bool Supports(IHasProviderIds item)
        {
            return item is Movie || item is Series;
        }
    }
}