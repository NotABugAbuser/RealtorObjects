using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RealtorObjects.View.Converters
{
    class StringMultiplierToNumericMultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if ((double)value == 1000000)
                return "Млн. руб";
            else
                return "Тыс. руб";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if ((string)value == "Тыс. руб")
                return 1000;
            else
                return 1000000;
        }
    }
}
