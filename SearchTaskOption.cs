using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbooruPlugin.Saucenao
{
    public class SearchTaskOption
    {
        public SaucenaoAPIOption APIOption { get; set; } = new SaucenaoAPIOption();

        public bool EnableDownload { get; set; }
        public string DownloadSavePath { get; set; }
        public string DownloadFileFormat { get; set; }
        public bool EnableCompressUploadFile { get; set; }
    }
}
