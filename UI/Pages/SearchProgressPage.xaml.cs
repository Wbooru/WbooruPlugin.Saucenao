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
using System.Windows.Media.Animation;
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

        enum LayoutState
        {
            One, Two, Three
        }

        LayoutState current_layout = LayoutState.One;

        private Storyboard layout_translate_storyboard;

        public ObservableCollection<SearchInstance> ProcessedInstance { get; set; } = new ObservableCollection<SearchInstance>();

        public static readonly DependencyProperty CurrentTaskProperty =
            DependencyProperty.Register("CurrentTask", typeof(SearchTask), typeof(SearchProgressPage), new PropertyMetadata((e,d)=>(e as SearchProgressPage)?.OnTaskChanged(d.OldValue as SearchTask, d.NewValue as SearchTask)));

        public SearchInstance CurrentSearchInstance
        {
            get { return (SearchInstance)GetValue(CurrentSearchInstanceProperty); }
            set { SetValue(CurrentSearchInstanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentSearchInstance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentSearchInstanceProperty =
            DependencyProperty.Register("CurrentSearchInstance", typeof(SearchInstance), typeof(SearchProgressPage), new PropertyMetadata(null));

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
            
            layout_translate_storyboard = new Storyboard();
            layout_translate_storyboard.Completed += (e, d) =>
            {
                ViewPage_SizeChanged(null, null);
                ObjectPool<ThicknessAnimation>.Return(e as ThicknessAnimation);
            };
        }

        private async void OnProcessedReport(IEnumerable<SearchInstance> obj)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                foreach (var instance in obj)
                {
                    ProcessedInstance.Add(instance);

                    RaiseProgressMessage($"Instance {instance.ImagePath} is done.");
                }

                ProcessCount += obj.Count();
            });
        }

        private void OnProgressReport(string obj) => RaiseProgressMessage(obj);

        private async void RaiseProgressMessage(string message) => await Dispatcher.InvokeAsync(() => MessageList.Text += message + Environment.NewLine);

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

        public void OnNavigationForwardAction()
        {
            switch (current_layout)
            {
                case LayoutState.One:
                    MenuButton_Click_2(null, null);
                    break;
                default:
                    break;
            }
        }

        public void OnNavigationBackAction()
        {
            switch (current_layout)
            {
                case LayoutState.One:
                    MenuButton_Click(null, null);
                    break;
                case LayoutState.Two:
                    MenuButton_Click_1(null, null);
                    break;
                case LayoutState.Three:
                    MenuButton_Click_2(null, null);
                    break;
                default:
                    break;
            }
        }

        private void ApplyTranslate()
        {
            layout_translate_storyboard.Children.Clear();

            if (ObjectPool<ThicknessAnimation>.Get(out var animation))
            {
                //init 
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(250));
                animation.FillBehavior = FillBehavior.Stop;
                Storyboard.SetTargetProperty(animation, new PropertyPath(Grid.MarginProperty));
                animation.EasingFunction = animation.EasingFunction ?? new QuadraticEase() { EasingMode = EasingMode.EaseOut };
            }

            animation.To = CalculateMargin();

            layout_translate_storyboard.Children.Clear();
            layout_translate_storyboard.Children.Add(animation);
            layout_translate_storyboard.Begin(MainPanel);
        }

        private void ViewPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var new_margin = CalculateMargin();
            MainPanel.Margin = new_margin;
        }

        private Thickness CalculateMargin()
        {
            double margin_left = 0;

            switch (current_layout)
            {
                case LayoutState.One:
                    margin_left = 0;
                    break;
                case LayoutState.Two:
                    margin_left = 1;
                    break;
                case LayoutState.Three:
                    margin_left = 2;
                    break;
                default:
                    break;
            }

            margin_left *= -ViewPage.ActualWidth;

            return new Thickness(margin_left, 0, 0, 0);
        }

        private void MenuButton_Click_1(object sender, RoutedEventArgs e)
        {
            current_layout = LayoutState.One;
            ApplyTranslate();
        }

        private void MenuButton_Click_2(object sender, RoutedEventArgs e)
        {
            current_layout = LayoutState.Two;
            ApplyTranslate();
        }

        private void MenuButton_Click_3(object sender, RoutedEventArgs e)
        {
            current_layout = LayoutState.Two;
            ApplyTranslate();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            e.Handled = true;

            if (!((sender as Hyperlink)?.DataContext is SearchInstance instance))
                return;

            CurrentSearchInstance = instance;

            current_layout = LayoutState.Three;
            ApplyTranslate();
        }

        private void Hyperlink_RequestNavigate_1(object sender, RequestNavigateEventArgs e)
        {
            e.Handled = true;

            if (!((sender as Hyperlink)?.NavigateUri is Uri uri))
                return;

            Process.Start(uri.AbsoluteUri);
        }
    }
}
