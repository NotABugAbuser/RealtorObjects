using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RealtorObjects.View.Converters
{
    class NameToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String name = value as String;
            return name == "Директор" || name == "Админ" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
