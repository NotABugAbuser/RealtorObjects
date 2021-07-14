using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;


namespace RealtorObjects.View.Converters
{
    class PageToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue) {
                return false;
            } else {
                string pageString = System.Convert.ToString(values[0]);
                if (pageString != "..." && pageString != "<" && pageString != ">") {
                    return System.Convert.ToInt16(values[0]) == System.Convert.ToInt16(values[1]);
                } else {
                    return false;
                }
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
