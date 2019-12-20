using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wbooru;
using Wbooru.Kernel;
using Wbooru.Settings;
using Wbooru.Utils;
using WbooruPlugin.Saucenao.Utils;

namespace WbooruPlugin.Saucenao.UI.Pages
{
    /// <summary>
    /// MainSearchImagePage.xaml 的交互逻辑
    /// </summary>
    public partial class MainSearchImagePage : Page
    {
        public MainSearchImagePage()
        {
            InitializeComponent();

            APIKeyInput.Text = SettingManager.LoadSetting<SaucenaoSetting>().SaucenaoAPIKey;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationHelper.NavigationPop();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //todo:加载未完成的
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.Title = "加载未完成的";

            if (dialog.ShowDialog()??false)
            {
                var load_file = dialog.FileName;

                if (string.IsNullOrWhiteSpace(load_file) || !File.Exists(load_file))
                    return;

                var task = JsonConvert.DeserializeObject<SearchTask>(File.ReadAllText(load_file));
                task.FilePath = load_file;

                BeginTask(task);
            }
        }

        private void BeginTask(SearchTask task)
        {
            SaucenaoSearch.BeginTask(task);
            //open search progress page
            NavigationHelper.NavigationPush(new SearchProgressPage(task));
        }

        private void Hyperlink_RequestNavigate_1(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void InputTextBox_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            InputTextBox.Text +=
                (InputTextBox.Text.EndsWith("\n") || InputTextBox.Text.Length == 0 || InputTextBox.Text.EndsWith("\r") ? string.Empty : Environment.NewLine)
                + string.Join(Environment.NewLine, files);

            if (files.Length!=0)
                InputTextBox.Text += Environment.NewLine;
        }

        private void InputTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!TryGetOption(out var option))
                return;

            var task = new SearchTask();

            var paths = InputTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .AsParallel()
                .Select(x => Directory.Exists(x) ? Directory.EnumerateFiles(x) : Enumerable.Repeat(x, 1))
                .SelectMany(x => x)
                .Where(x => FastImageFormatChecker.IsImageFormat(x))
                .ToArray();

            paths.ForEach(x => Log.Debug($"Add saucenao search path: {x}"));

            task.Options = option;
            task.SearchImageInstances = paths.Select(x => new SearchInstance()
            {
                ImagePath = x
            });

            if (paths.Length == 0)
                return;

            AskIfNeedSave(task);

            BeginTask(task);
        }

        private void AskIfNeedSave(SearchTask task)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Title = "(可选) 选择文件保存路径",
                CreatePrompt = true,
            };

            if (dialog.ShowDialog()??false)
            {
                task.FilePath = dialog.FileName;
                SaucenaoSearch.SaveTask(task);
            }
        }

        private bool TryGetOption(out SearchTaskOption o)
        {
            o = null;
            var option = new SearchTaskOption();

            if (string.IsNullOrWhiteSpace(APIKeyInput.Text))
                return false;
            option.APIOption.APIKey = APIKeyInput.Text;

            if (!int.TryParse(DataBaseIndexInput.Text,out var db))
                return false;
            option.APIOption.DB = db;

            if (!int.TryParse(RequestCountInput.Text, out var rc))
                return false;
            option.APIOption.RequestCount = rc;

            option.APIOption.DBMask = int.TryParse(DBMaskInput.Text, out var mask) ? mask : default(int?);
            option.APIOption.DBMaski = int.TryParse(DBMaskiInput.Text, out var maski) ? maski : default(int?);

            option.APIOption.TestMode = TestModeCheckBox.IsChecked ?? false;

            option.EnableCompressUploadFile = CompressCheckBox.IsChecked ?? false;

            option.EnableDownload = EnableDownloadCheckBox.IsChecked ?? false;
            option.DownloadFileFormat = DownloadFileFormatInput.Text;
            option.DownloadSavePath = DownloadSavePathInput.Text;

            o = option;
            return true;
        }
    }
}
