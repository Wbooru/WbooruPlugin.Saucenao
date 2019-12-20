using System;
using System.Collections.Generic;
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

namespace WbooruPlugin.Saucenao.UI.Pages
{
    /// <summary>
    /// SearchProgressPage.xaml 的交互逻辑
    /// </summary>
    public partial class SearchProgressPage : Page
    {
        public SearchTask CurrentTask
        {
            get { return (SearchTask)GetValue(CurrentTaskProperty); }
            set { SetValue(CurrentTaskProperty, value); }
        }

        public static readonly DependencyProperty CurrentTaskProperty =
            DependencyProperty.Register("CurrentTask", typeof(SearchTask), typeof(SearchProgressPage), new PropertyMetadata(null));

        public SearchProgressPage(SearchTask task)
        {
            InitializeComponent();

            task = task ?? SearchTask.CurrentSearchTask;
            Debug.Assert(task != null);

            CurrentTask = task;

            MainPanel.DataContext = this;
        }
    }
}
