using FileSignatures;
using FileSignatures.Formats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WbooruPlugin.Saucenao.Utils
{
    public static class FastImageFormatChecker
    {
        static FastImageFormatChecker()
        {
            var recognised = FileFormatLocator.GetFormats().OfType<Image>();
            Checker = new FileFormatInspector(recognised);
        }

        public static FileFormatInspector Checker { get; }

        public static bool IsImageFormat(string path)
        {
            using var stream = File.OpenRead(path);

            return Checker.DetermineFileFormat(stream) is Image;
        }
    }
}
