using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbooruPlugin.Saucenao.Utils
{
    public static class UnicodeEncodingHelper
    {
        public static string DecodeFromUnicodeText(string text)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var split in text.Split(new string[] { "\\u" }, StringSplitOptions.None))
            {

            }

            return null;
        }
    }
}
