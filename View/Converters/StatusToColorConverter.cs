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
    class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            Status status = (Status)value;
            if (status == Status.Active)
                return new SolidColorBrush(Color.FromRgb(107, 190, 111));
            else if (status == Status.Planned)
                return new SolidColorBrush(Color.FromRgb(216, 180, 0));
            else
                return new SolidColorBrush(Color.FromRgb(153, 0, 0));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
