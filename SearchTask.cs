using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WbooruPlugin.Saucenao
{
    /// <summary>
    /// searializable
    /// </summary>
    public class SearchTask : INotifyPropertyChanged
    {
        public static SearchTask CurrentSearchTask { get; internal set; }

        public SearchTaskOption Options { get; set; }

        public IEnumerable<SearchInstance> SearchImageInstances { get; set; }

        internal bool IsProcessing { get; set; }
        internal (Task, CancellationTokenSource) ProcessingTask { get; set; }
        internal string FilePath { get; set; }
        internal Action<string> ProgressReporter { get; set; } = null;
        internal Action<IEnumerable<SearchInstance>> ProcessedReporter { get; set; } = null;

        public void Report(string message) => ProgressReporter?.Invoke(message);
        public void Report(params SearchInstance[] processed_instances) => ProcessedReporter?.Invoke(processed_instances);

        private SearchInstance current_processing_instance;

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchInstance CurrentProcessingInstance
        {
            get => current_processing_instance;
            set
            {
                current_processing_instance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentProcessingInstance)));
            }
        }
    }
}
