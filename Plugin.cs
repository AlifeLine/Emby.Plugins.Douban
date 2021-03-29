using System;
using System.Collections.Generic;
using MediaBrowser.Common.Plugins;
using Emby.Plugins.Douban.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
namespace Emby.Plugins.Douban
{
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public const string PluginName = "Douban Emby";

        public override Guid Id => new Guid("ce69a5ea-14b6-44a3-b75a-9d21dd32a7cf");

        public override string Name => PluginName;

        public override string Description => "Improved Metadata Provider, specifically designed for Douban.";

        public static Plugin Instance { get; private set; }
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    Name = this.Name,
                    EmbeddedResourcePath = string.Format("{0}.Configuration.configPage.html", GetType().Namespace)
                }
            };
        }
    }
}