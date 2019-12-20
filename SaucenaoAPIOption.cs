namespace WbooruPlugin.Saucenao
{
    public class SaucenaoAPIOption
    {
        public string APIKey { get; set; }

        public bool TestMode { get; set; }
        public int? DBMask { get; set; }
        public int? DBMaski { get; set; }

        public int DB { get; set; }

        public int RequestCount { get; set; }
    }
}