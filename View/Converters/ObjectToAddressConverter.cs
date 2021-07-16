using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RealtorObjects.View.Converters
{
    class ObjectToAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            BaseRealtorObject realtorObject = value as BaseRealtorObject;
            StringBuilder builder = new StringBuilder();
            builder.Append(realtorObject.Location.City);
                builder.Append("   •   ");
            builder.Append(realtorObject.Location.District);
                builder.Append("   •   улица ");
            builder.Append(realtorObject.Location.Street);
                builder.Append("   •   д. ");
            builder.Append(realtorObject.Location.HouseNumber);
            if (realtorObject is Flat flat){
                builder.Append("   •   кв. ");
                builder.Append(flat.Location.FlatNumber);
            }
            return builder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
