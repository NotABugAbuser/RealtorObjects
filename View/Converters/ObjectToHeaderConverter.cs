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
    class ObjectToHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            StringBuilder builder = new StringBuilder();
            BaseRealtorObject realtorObject = value as BaseRealtorObject;
            if (realtorObject.GeneralInfo.ObjectType == "Комната") {
                builder.Append($"Комната {realtorObject.GeneralInfo.General} м², {realtorObject.GeneralInfo.CurrentLevel}/{realtorObject.GeneralInfo.CurrentLevel} этаж");
            } else if (realtorObject.GeneralInfo.ObjectType == "Квартира") {
                builder.Append($"{realtorObject.GeneralInfo.RoomCount}-комн квартира {realtorObject.GeneralInfo.General} м², {realtorObject.GeneralInfo.CurrentLevel}/{realtorObject.GeneralInfo.CurrentLevel} этаж");
            } else if (realtorObject.GeneralInfo.ObjectType == "Дом") {
                builder.Append($"Дом {realtorObject.GeneralInfo.General} м², {realtorObject.GeneralInfo.LevelCount} ");
                int remainder = realtorObject.GeneralInfo.LevelCount % 10;
                if (remainder == 1) {
                    builder.Append("этаж");
                } else if (remainder == 2 || remainder == 3 || remainder == 4) {
                    builder.Append("этажа");
                } else {
                    builder.Append("этажей");
                }
            } else {
                builder.Append($"Земельный участок, {(realtorObject as House).Info.Hundreds} сот.");
            }
            return builder.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
