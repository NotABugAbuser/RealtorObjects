using RealtyModel.Model.Base;
using RealtyModel.Model.RealtyObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace RealtorObjects.View.Converters
{
    class StatusToBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            Status status = (Status)value;
            if (status == Status.Active)
                return new SolidColorBrush(Color.FromRgb(209, 234, 210));
            else if (status == Status.Planned)
                return new SolidColorBrush(Color.FromRgb(255, 255, 150));
            else
                return new SolidColorBrush(Color.FromRgb(255, 153, 147));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
