using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wbooru.Kernel.Updater;
using Wbooru.PluginExt;

namespace WbooruPlugin.Saucenao
{
    [Export(typeof(PluginInfo))]
    public class SaucenaoPluginInfo : PluginInfo , IPluginUpdatable
    {
        public override string PluginName => "Saucenao Image Infomation Search Plugin";

        public override string PluginProjectWebsite => "https://github.com/MikiraSora/WbooruPlugin.Saucenao";

        public override string PluginAuthor => "MikiraSora";

        public override string PluginDescription => "这里可以通过Saucenao提供的api，来搜索图片相关信息。";

        public Version CurrentPluginVersion => GetType().Assembly.GetName().Version;

        public IEnumerable<ReleaseInfo> GetReleaseInfoList()
        {
            return Enumerable.Empty<ReleaseInfo>();
        }
    }
}
