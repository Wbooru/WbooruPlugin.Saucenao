using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using Wbooru.Kernel;
using Wbooru.Utils;

namespace WbooruPlugin.Saucenao.UI.Pages
{
    /// <summary>
    /// SearchProgressPage.xaml 的交互逻辑
    /// </summary>
    public partial class SearchProgressPage : Page , INavigatableAction
    {
        public SearchTask CurrentTask
        {
            get { return (SearchTask)GetValue(CurrentTaskProperty); }
            set { SetValue(CurrentTaskProperty, value); }
        }

        public ObservableCollection<SearchInstance> ProcessedInstance { get; set; } = new ObservableCollection<SearchInstance>();

        public static readonly DependencyProperty CurrentTaskProperty =
            DependencyProperty.Register("CurrentTask", typeof(SearchTask), typeof(SearchProgressPage), new PropertyMetadata((e,d)=>(e as SearchProgressPage)?.OnTaskChanged(d.OldValue as SearchTask, d.NewValue as SearchTask)));

        public int ProcessCount
        {
            get { return (int)GetValue(ProcessCountProperty); }
            set { SetValue(ProcessCountProperty, value); }
        }

        public static readonly DependencyProperty ProcessCountProperty =
            DependencyProperty.Register("ProcessCount", typeof(int), typeof(SearchProgressPage), new PropertyMetadata(0));

        public int TaskCount
        {
            get { return (int)GetValue(TaskCountProperty); }
            set { SetValue(TaskCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TaskCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskCountProperty =
            DependencyProperty.Register("TaskCount", typeof(int), typeof(SearchProgressPage), new PropertyMetadata(0));

        private void OnTaskChanged(SearchTask old_task,SearchTask task)
        {
            ProcessedInstance.Clear();

            if (old_task != null)
            {
                old_task.ProgressReporter = null;
                old_task.ProcessedReporter = null;
            }

            //bind task callback
            if (task != null)
            {
                task.ProgressReporter = OnProgressReport;
                task.ProcessedReporter = OnProcessedReport;

                TaskCount = task.SearchImageInstances.Count();
            }
        }

        public SearchProgressPage(SearchTask task)
        {
            InitializeComponent();

            task = task ?? SearchTask.CurrentSearchTask;
            Debug.Assert(task != null);

            CurrentTask = task;
        }

        private void OnProcessedReport(IEnumerable<SearchInstance> obj)
        {
            foreach (var instance in obj)
            {
                ProcessedInstance.Add(instance);

                RaiseProgressMessage($"Instance {instance.ImagePath} is done.");
            }

            ProcessCount += obj.Count();
        }

        private void OnProgressReport(string obj) => RaiseProgressMessage(obj);

        private void RaiseProgressMessage(string message) => MessageList.Text += message + Environment.NewLine;

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            //check if task has been done.
            if (TaskCount < ProcessCount)
                AskApplyBackground();
            else
                NavigationHelper.NavigationPop();
        }

        private void AskApplyBackground()
        {

        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!((sender as FrameworkElement).DataContext is string path) || string.IsNullOrWhiteSpace(path))
                return;

            try
            {
                Process.Start(path);
            }
            catch (Exception ex)
            {
                ExceptionHelper.DebugThrow(ex);
                //igonore this
            }
        }

        public void OnNavigationBackAction()
        {
            throw new NotImplementedException();
        }

        public void OnNavigationForwardAction()
        {
            throw new NotImplementedException();
        }
    }
}
