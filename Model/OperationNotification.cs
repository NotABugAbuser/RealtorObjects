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
using System.Windows.Threading;

namespace RealtorObjects.Model
{
    public static class OperationNotification
    {
        private static readonly Dictionary<ErrorCode, NotificationInfo> codes = new Dictionary<ErrorCode, NotificationInfo>()
        {
            [ErrorCode.Successful] = new NotificationInfo("Операция завершена успешно", CodeType.Successful),
            [ErrorCode.Credential] = new NotificationInfo("Введены неверные данные", CodeType.Exclamation),
            [ErrorCode.Unknown] = new NotificationInfo("Операция не завершена", CodeType.Error),
            [ErrorCode.WrongData] = new NotificationInfo("Введены неверные данные", CodeType.Exclamation),
            [ErrorCode.NoRequiredData] = new NotificationInfo("Запрошенная информация отсутствует в базе данных", CodeType.Exclamation),
            [ErrorCode.FlatAddedSuccessfuly] = new NotificationInfo("Квартира добавлена успешно", CodeType.Successful),
            [ErrorCode.NoLocations] = new NotificationInfo("В базе данных нет локаций", CodeType.Exclamation),
            [ErrorCode.ServerUnavailable] = new NotificationInfo("Связь с сервером отсутствует", CodeType.Error),
            [ErrorCode.NoRealtorObjects] = new NotificationInfo("В базе данных нет объектов", CodeType.Exclamation),
            [ErrorCode.WrongAgent] = new NotificationInfo("У вас нет прав на редактирование этого объекта.", CodeType.Exclamation),
            [ErrorCode.FlatUpdatedSuccessfuly] = new NotificationInfo("Изменения квартиры успешно занесены в базу данных", CodeType.Successful),
            [ErrorCode.FlatDuplicate] = new NotificationInfo("Квартира с данным адресом уже существует в базе данных", CodeType.Exclamation)
        };
        public static void Notify(ErrorCode code)
        {
            if (code != ErrorCode.NoCode)
            {
                NotificationWindow notification = new NotificationWindow(codes[code]);
                notification.Show();
                notification.Left = SystemParameters.WorkArea.Width - notification.ActualWidth;
                notification.Top = SystemParameters.WorkArea.Height - notification.ActualHeight;
            }
        }
    }
}
