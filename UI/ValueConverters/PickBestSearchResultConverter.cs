using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace WbooruPlugin.Saucenao.UI.ValueConverters
{
    public class PickBestSearchResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is SearchInstance instance))
                return null;

            var result = instance.ResultInfos.OrderByDescending(x => x.Header.Similarity).FirstOrDefault();

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
