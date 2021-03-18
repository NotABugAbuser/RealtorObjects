using RealtorObjects.Model;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using RealtyModel.Event;
using RealtorObjects.View;
using System.Threading;
using System.IO;

namespace RealtorObjects.ViewModel
{
    public class LoginFormViewModel : BaseViewModel, ILogged
    {
        private CustomCommand closeApp;
        private CustomCommand sendPassword;
        private CustomCommand changeRegistrationVisibility;
        private CustomCommand createNewUser;
        private CustomCommand login;

        private string currentLogin = "";
        private string currentPassword = "";
        private string name = "";
        private string surname = "";
        private string patronymic = "";
        private string email = "";
        private string firstPassword = "";
        private string secondPassword = "";
        private Visibility registrationVisibility = Visibility.Collapsed;

        public LoginFormViewModel()
        {

        }

        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj =>
        {
            Application.Current.Shutdown();
        }));
        public CustomCommand SendPassword => sendPassword ?? (sendPassword = new CustomCommand(obj =>
        {

        }));
        public CustomCommand ChangeRegistrationVisibility => changeRegistrationVisibility ?? (changeRegistrationVisibility = new CustomCommand(obj =>
        {
            if (RegistrationVisibility == Visibility.Visible)
            {
                RegistrationVisibility = Visibility.Collapsed;
            }
            else
            {
                RegistrationVisibility = Visibility.Visible;
            }
        }));
        public CustomCommand CreateNewUser => createNewUser ?? (createNewUser = new CustomCommand(obj =>
        {
            CurrentLogin = $"{Surname}{Name[0]}{Patronymic[0]}";
            Client client = ((App)Application.Current).Client;
            Operation operation = new Operation()
            {
                Name = CurrentLogin,
                Data = SecondPassword, //Нужна кодировка
                OperationParameters = new OperationParameters()
                {
                    Direction = OperationDirection.Identity,
                    Type = OperationType.Register
                }
            };
            client.SendMessage(operation);

            for(byte attempts = 0; attempts <= 30; attempts++)
            {
                if (client.IncomingOperations.Count > 0)
                {
                    Operation incomnigOperation = client.IncomingOperations.Dequeue();
                    if (incomnigOperation.OperationParameters.Direction == OperationDirection.Identity 
                    && incomnigOperation.OperationParameters.Type == OperationType.Register 
                    && incomnigOperation.Name == CurrentLogin 
                    && incomnigOperation.IsSuccessfully
                    )
                    {
                        MessageBox.Show("Регистрация прошла успешно");
                        RegistrationVisibility = Visibility.Collapsed;
                        break;
                    }
                }
                if (attempts == 30) MessageBox.Show("Что-то пошло не так");
                Thread.Sleep(100);
            }
        }, obj =>
        { // проверяет поля формы на заполненность НАДО ПЕРЕДЕЛАТЬ!!!
            return !(String.IsNullOrEmpty(Name)
            && String.IsNullOrEmpty(Surname)
            && String.IsNullOrEmpty(Patronymic)
            && String.IsNullOrEmpty(Email)
            && String.IsNullOrEmpty(FirstPassword)
            && String.IsNullOrEmpty(SecondPassword));
        }));
        public CustomCommand Login => login ?? (login = new CustomCommand(obj =>
        {
            Client client = ((App)Application.Current).Client;
            Operation operation = new Operation()
            {
                Name = CurrentLogin,
                OperationParameters = new OperationParameters()
                {
                    Direction = OperationDirection.Identity,
                    Type = OperationType.Login
                },
                Data = currentPassword
            };
            client.SendMessage(operation);

            for(byte attempts = 0; attempts <= 30; attempts++)
            {
                if (client.IncomingOperations.Count > 0)
                {
                    Operation incomnigOperation = client.IncomingOperations.Dequeue();
                    if (
                    incomnigOperation.OperationParameters.Direction == OperationDirection.Identity
                    && incomnigOperation.OperationParameters.Type == OperationType.Login
                    && incomnigOperation.Name == CurrentLogin
                    && incomnigOperation.IsSuccessfully
                    )
                    {
                        Logged?.Invoke(this, new LoggedEventArgs(obj));
                        break;
                    }
                    if (attempts == 30) MessageBox.Show("Что-то пошло не так");
                }
                Thread.Sleep(100);
            }
        }));

        public string CurrentLogin
        {
            get => currentLogin;
            set
            {
                currentLogin = value;
                OnPropertyChanged();
            }
        }
        public string CurrentPassword
        {
            get => currentPassword;
            set
            {
                currentPassword = value;
                OnPropertyChanged();
            }
        }
        public Visibility RegistrationVisibility
        {
            get => registrationVisibility;
            set
            {
                registrationVisibility = value;
                OnPropertyChanged();
            }
        }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }
        public string Surname
        {
            get => surname;
            set
            {
                surname = value;
                OnPropertyChanged();
            }
        }
        public string Patronymic
        {
            get => patronymic;
            set
            {
                patronymic = value;
                OnPropertyChanged();
            }
        }
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }
        public string FirstPassword
        {
            get => firstPassword;
            set
            {
                firstPassword = value;
                OnPropertyChanged();
            }
        }
        public string SecondPassword
        {
            get => secondPassword;
            set
            {
                secondPassword = value;
                OnPropertyChanged();
            }
        }

        public event LoggedEventHandler Logged;
    }
}
