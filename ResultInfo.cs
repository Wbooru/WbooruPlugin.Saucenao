using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace WbooruPlugin.Saucenao
{
    public class ResultInfo
    {
        public class HeaderInfo
        {
            [JsonProperty("similarity")]
            public float Similarity { get; set; }

            [JsonProperty("index_id")]
            public int IndexID { get; set; }

            public string DatabaseName => Enum.TryParse<DatabaseEnum>(IndexID.ToString(), out var e) ? e.ToString() : string.Empty;

            [JsonProperty("index_name")]
            public string IndexName { get; set; }

            [JsonProperty("thumbnail")]
            public string ThumbnailUrl { get; set; }
        }

        [JsonProperty("data")]
        public JToken DataJson { get; set; }

        [JsonProperty("header")]
        public HeaderInfo Header { get; set; }

        private string title = "";

        public string AvaliableDisplayName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(title))
                    return title;

                title = DataJson["title"]?.ToString();

                title = string.IsNullOrWhiteSpace(title) ? Header.IndexName : title;

                return title;
            }
        }

        private string reference_url = "";

        public string ReferenceURL
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(reference_url))
                    return reference_url;

                reference_url = DataJson["ext_urls"]?.First?.ToString();

                return reference_url;
            }
        }
    }
}