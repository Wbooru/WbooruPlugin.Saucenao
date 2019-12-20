using System.Collections.Generic;

namespace WbooruPlugin.Saucenao
{
    public class SearchInstance
    {
        public string ImagePath { get; set; }

        public IEnumerable<ResultInfo> ResultInfos { get; set; }
    }
}