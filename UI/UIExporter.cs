using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Wbooru;
using Wbooru.Kernel;
using Wbooru.UI.Controls;
using Wbooru.UI.Controls.PluginExtension;
using WbooruPlugin.Saucenao.UI.Pages;

namespace WbooruPlugin.Saucenao.UI
{
    internal static class UIExporter
    {
        private static bool _init = false;

        public static void ExportContent()
        {
            if (_init)
                return;
            _init = true;

            var menu_button = new MenuButton()
            {
                Name = "SaucenaoSearcherMenuButton",
                IconSize=15,
                Text = "搜图",
                Icon = ""
            };

            menu_button.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnMenuButtonClick));

            Wbooru.Container.Default.ComposeExportedValue<IExtraMainMenuItem>(menu_button);
        }

        private static void OnMenuButtonClick(object sender, RoutedEventArgs e)
        {
            NavigationHelper.NavigationPush(new MainSearchImagePage());
        }
    }
}
