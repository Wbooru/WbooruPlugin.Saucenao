using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wbooru.Settings;

namespace WbooruPlugin.Saucenao
{
    [Export(typeof(SettingBase))]
    public class SaucenaoSetting:SettingBase
    {
        public string SaucenaoAPIKey { get; set; }
    }
}
