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

        public String Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        public String Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }
        public String RepeatedPassword
        {
            get => repeatedPassword;
            set
            {
                repeatedPassword = value;
                OnPropertyChanged();
            }
        }
        public ICommand LoginCommand { get; private set; }
        public ICommand RegisterCommand { get; private set; }

        public AuthentificationViewModel()
        {
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
                Client.SendMessage(login);
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
                    Client.SendMessage(register);
                }
                else UpdateLog("Passwords doesn't match");
            });
        }
    }
}
