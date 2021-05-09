using RealtyModel.Model.Base;
using RealtyModel.Model.RealtyObjects;
using System;
using System.Globalization;
using System.Windows.Data;

namespace RealtorObjects.View.Converters
{
    class StatusEngToRusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            Status status = (Status)value;
            if (status == Status.Active)
                return "Активный";
            else if (status == Status.Planned)
                return "В планах";
            else
                return "Архивный";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
