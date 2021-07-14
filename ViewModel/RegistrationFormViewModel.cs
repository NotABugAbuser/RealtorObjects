using RealtorObjects.Model;
using RealtorObjects.View;
using RealtyModel.Model;
using RealtyModel.Model.Operations;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    class RegistrationFormViewModel : BaseViewModel
    {
        private CredentialData credentials = new CredentialData();
        private CustomCommand register;
        private CustomCommand cancel;
        public CustomCommand Register => register ?? (register = new CustomCommand(obj =>
        {
            bool hasEmptyFields = String.IsNullOrWhiteSpace(credentials.Name) ||
                                  String.IsNullOrWhiteSpace(credentials.Surname) ||
                                  String.IsNullOrWhiteSpace(credentials.Patronymic) ||
                                  String.IsNullOrWhiteSpace(credentials.FirstPassword) ||
                                  String.IsNullOrWhiteSpace(credentials.SecondPassword) ||
                                  String.IsNullOrWhiteSpace(credentials.Email);
            if (!hasEmptyFields)
            {
                if (credentials.FirstPassword.Equals(credentials.SecondPassword))
                {
                    Credential current = new Credential();
                    current.Name = $"{credentials.Surname}{credentials.Name[0]}{credentials.Patronymic[0]}";
                    current.RegistrationDate = DateTime.Now;
                    current.Password = credentials.SecondPassword;
                    current.Email = credentials.Email;
                    ErrorCode code = Client.Register(current);
                    if (code == ErrorCode.Successful)
                    {
                        OperationNotification.SuccessfulNotify("Агент зарегистрирован");
                        (obj as Window).Close();
                    }
                    else if (code == ErrorCode.AgentExists) OperationNotification.WarningNotify("Агент не зарегистрирован");
                    else OperationNotification.WarningNotify("Что-то пошло не так");
                }
                else OperationNotification.WarningNotify("Пароли не совпадают");
            }
            else OperationNotification.WarningNotify("Не все поля заполнены");
        }));
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj => (obj as Window).Close()));
        public CredentialData Credentials
        {
            get => credentials;
            set => credentials = value;
        }
    }
}
