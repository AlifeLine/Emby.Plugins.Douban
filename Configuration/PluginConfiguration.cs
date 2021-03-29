using MediaBrowser.Model.Plugins;

namespace Emby.Plugins.Douban.Configuration
{
    public class PluginConfiguration : BasePluginConfiguration
    {
        public string ApiKey { get; set; }
        public int MinRequestInternalMs { get; set; }
        public PluginConfiguration()
        {
            MinRequestInternalMs = 2000;
        }
    }
}