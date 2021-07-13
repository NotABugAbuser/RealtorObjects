using RealtorObjects.View;
using RealtyModel.Model.Operations;
using System.Collections.Generic;
using System.Windows;

namespace RealtorObjects.Model
{
    public static class OperationNotification
    {
        private static readonly Dictionary<ErrorCode, NotificationInfo> codes = new Dictionary<ErrorCode, NotificationInfo>() {
            [ErrorCode.Successful] = new NotificationInfo("Операция завершена успешно", CodeType.Successful),
            [ErrorCode.Credential] = new NotificationInfo("Введены неверные данные", CodeType.Exclamation),
            [ErrorCode.Unknown] = new NotificationInfo("Операция не завершена", CodeType.Error),
            [ErrorCode.WrongData] = new NotificationInfo("Введены неверные данные", CodeType.Exclamation),
            [ErrorCode.NoRequiredData] = new NotificationInfo("Запрошенная информация отсутствует в базе данных", CodeType.Exclamation),
            [ErrorCode.ServerUnavailable] = new NotificationInfo("Связь с сервером отсутствует", CodeType.Error),
            [ErrorCode.NoRealtorObjects] = new NotificationInfo("В базе данных нет объектов", CodeType.Exclamation),
            [ErrorCode.WrongAgent] = new NotificationInfo("У вас нет прав на редактирование этого объекта.", CodeType.Exclamation),
            [ErrorCode.FlatUpdatedSuccessfuly] = new NotificationInfo("Изменения квартиры успешно занесены в базу данных", CodeType.Successful),
            [ErrorCode.ObjectDuplicate] = new NotificationInfo("Объект с данным адресом уже существует в базе данных", CodeType.Exclamation),
            [ErrorCode.WrongFormat] = new NotificationInfo("Введены данные в неверном формате", CodeType.Error),
            [ErrorCode.AgentExists] = new NotificationInfo("Такой агент уже существует", CodeType.Exclamation),
            [ErrorCode.ObjectDuplicate] = new NotificationInfo("Объект с таким адресом уже существует", CodeType.Exclamation),
            [ErrorCode.ObjectUpdatedSuccessfuly] = new NotificationInfo("Изменения объекта успешно занесены в базу данных", CodeType.Successful)
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
        public static void SuccessfulNotify(ErrorCode code, string message)
        {
            if (code != ErrorCode.NoCode)
            {
                NotificationWindow notification = new NotificationWindow(new NotificationInfo(message, CodeType.Successful));
                notification.Show();
                notification.Left = SystemParameters.WorkArea.Width - notification.ActualWidth;
                notification.Top = SystemParameters.WorkArea.Height - notification.ActualHeight;
            }
        }
        public static void WarningNotify(ErrorCode code, string message)
        {
            if (code != ErrorCode.NoCode)
            {
                NotificationWindow notification = new NotificationWindow(new NotificationInfo(message, CodeType.Exclamation));
                notification.Show();
                notification.Left = SystemParameters.WorkArea.Width - notification.ActualWidth;
                notification.Top = SystemParameters.WorkArea.Height - notification.ActualHeight;
            }
        }
    }
}
