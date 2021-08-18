using MediaBrowser.Model.Plugins;

namespace Emby.Plugins.Douban.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string ApiKey { get; set; }
        public int MinRequestInternalMs { get; set; }
        public string doubanPhoneorEmail { get; set; }
        public string doubanPassword { get; set; }
        public bool isUserLogin { get; set; }
        public PluginConfiguration()
        {
            MinRequestInternalMs = 2000;
            isUserLogin = false;
        }
    }
}