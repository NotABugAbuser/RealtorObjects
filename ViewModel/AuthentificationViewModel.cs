using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using System.Windows.Threading;
using RealtyModel.Model;
using RealtyModel.Service;
using RealtorObjects.Model;

namespace RealtorObjects.ViewModel
{
    class AuthentificationViewModel : BaseViewModel
    {
        String name = "";
        String password = "";
        String repeatedPassword = "";
        Client client;

        /// <summary>
        /// Свойство имени(логина) пользователя. 
        /// </summary>
        public String Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Свойство пароля пользователя.
        /// </summary>
        public String Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Свойство повторного пароля пользователя, используемое при регистрации.
        /// </summary>
        public String RepeatedPassword
        {
            get => repeatedPassword;
            set
            {
                repeatedPassword = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Команда отправки запроса на авторизацию.
        /// </summary>
        public ICommand LoginCommand { get; private set; }
        /// <summary>
        /// Команда отправки запроса на регистрацию.
        /// </summary>
        public ICommand RegisterCommand { get; private set; }

        public AuthentificationViewModel(Client client, ObservableCollection<LogMessage> log) : base(log)
        {
            this.Log = log;
            this.client = client;
            LoginCommand = new CustomCommand((obj) =>
            {
                Operation login = new Operation()
                {
                    IpAddress = "",
                    OperationNumber = new Guid(),
                    OperationType = OperationType.Login,
                    Data = JsonSerializer.Serialize<Credential>
                    (
                        new Credential() { Name = this.Name, Password = this.Password }
                    )
                };
                client.SendMessage(login);
            });
            RegisterCommand = new CustomCommand((obj) =>
            {
                if (Password == RepeatedPassword)
                {
                    Operation register = new Operation()
                    {
                        IpAddress = "",
                        OperationNumber = new Guid(),
                        OperationType = OperationType.Register,
                        Data = JsonSerializer.Serialize<Credential>
                       (
                           new Credential() { Name = this.Name, Password = this.Password }
                       )
                    };
                    client.SendMessage(register);
                }
                else UpdateLog("Passwords doesn't match");
            });
        }
    }
}
