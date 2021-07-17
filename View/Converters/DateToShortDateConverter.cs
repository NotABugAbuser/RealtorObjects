using RealtyModel.Model.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RealtorObjects.View.Converters
{
    class DateToShortDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            BaseRealtorObject realtorObject = value as BaseRealtorObject;
            CultureInfo cultureInfo = new CultureInfo("ru-RU");
            DateTimeFormatInfo dtfi = cultureInfo.DateTimeFormat;
            return realtorObject.RegistrationDate.ToLongDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
