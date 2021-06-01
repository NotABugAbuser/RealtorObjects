using FontAwesome.WPF;
using RealtorObjects.View;
using RealtyModel.Model.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace RealtorObjects.Model
{
    public static class OperationNotification
    {
        private static Dictionary<ErrorCode, NotificationInfo> codes = new Dictionary<ErrorCode, NotificationInfo>() {
            [ErrorCode.Successful] = new NotificationInfo("Операция завершена успешно", CodeType.Successful),
            [ErrorCode.Credential] = new NotificationInfo("Введены неверные данные", CodeType.Exclamation),
            [ErrorCode.Unknown] = new NotificationInfo("Операция не завершена", CodeType.Error),
            [ErrorCode.WrongData] = new NotificationInfo("Введены неверные данные", CodeType.Exclamation),
            [ErrorCode.NoRequiredData] = new NotificationInfo("Запрошенная информация отсутствует в базе данных", CodeType.Exclamation),
            [ErrorCode.FlatAddedSuccessfuly] = new NotificationInfo("Квартира добавлена успешно", CodeType.Successful),
            [ErrorCode.NoLocations] = new NotificationInfo("В базе данных нет локаций", CodeType.Exclamation)
        };
        public static void Notify(ErrorCode code) {
            if (code != ErrorCode.NoCode) {
                new NotificationWindow(codes[code]).Show();
            }
        }
    }
}
