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
            [ErrorCode.WrongAgent] = new NotificationInfo("У вас нет прав на редактирование этого объекта.", CodeType.Exclamation),
            [ErrorCode.WrongFormat] = new NotificationInfo("Введены данные в неверном формате", CodeType.Error),
            [ErrorCode.AgentExists] = new NotificationInfo("Такой агент уже существует в базе данных. Операция отменена", CodeType.Exclamation),
            [ErrorCode.ObjectDuplicate] = new NotificationInfo("Объект с таким адресом уже существует в базе данных. Операция отменена", CodeType.Exclamation),
            [ErrorCode.ObjectUpdatedSuccessfuly] = new NotificationInfo("Изменения объекта успешно занесены в базу данных", CodeType.Successful),
            [ErrorCode.ObjectAddedSuccessfuly] = new NotificationInfo("Объект успешно занесен в базу данных", CodeType.Successful)
        };
        public static void Notify(ErrorCode code) {
            if (code != ErrorCode.NoCode) {
                NotificationWindow notification = new NotificationWindow(codes[code]);
                notification.Show();
                notification.Left = SystemParameters.WorkArea.Width - notification.ActualWidth;
                notification.Top = SystemParameters.WorkArea.Height - notification.ActualHeight;
            }
        }
        public static void SuccessfulNotify(string message) {
            NotificationWindow notification = new NotificationWindow(new NotificationInfo(message, CodeType.Successful));
            notification.Show();
            notification.Left = SystemParameters.WorkArea.Width - notification.ActualWidth;
            notification.Top = SystemParameters.WorkArea.Height - notification.ActualHeight;
        }

        public static void WarningNotify(string message) {
            NotificationWindow notification = new NotificationWindow(new NotificationInfo(message, CodeType.Exclamation));
            notification.Show();
            notification.Left = SystemParameters.WorkArea.Width - notification.ActualWidth;
            notification.Top = SystemParameters.WorkArea.Height - notification.ActualHeight;
        }
    }
}
