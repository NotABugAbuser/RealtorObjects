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
using System.Diagnostics;
using RealtyModel.Event.IdentityEvents;

namespace RealtorObjects.ViewModel
{
    public class LoginFormViewModel : BaseViewModel
    {
        private CustomCommand login;
        private CustomCommand closeApp;
        private CustomCommand sendPassword;
        private CustomCommand createNewUser;
        private CustomCommand changeRegistrationVisibility;
        private CredentialData credentials = new CredentialData();
        private Visibility registrationVisibility = Visibility.Collapsed;
        public event LoggingInEventHandler LoggingIn;
        public event RegisteringEventHandler Registering;

        public LoginFormViewModel()
        {
        }

        public CustomCommand Login => login ?? (login = new CustomCommand(obj =>
        {
            if (String.IsNullOrWhiteSpace(Credentials.CurrentUsername))
                MessageBox.Show("Введите логин");
            else if (String.IsNullOrWhiteSpace(Credentials.CurrentPassword))
                MessageBox.Show("Введите пароль");
            else
                LoggingIn?.Invoke(this, new LoggingInEventArgs(credentials.CurrentUsername, credentials.CurrentPassword));
        }));
        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj =>
        {
            Application.Current.Shutdown();
        }));
        public CustomCommand SendPassword => sendPassword ?? (sendPassword = new CustomCommand(obj =>
        {

        }));
        public CustomCommand CreateNewUser => createNewUser ?? (createNewUser = new CustomCommand(obj =>
        {
            if (String.IsNullOrWhiteSpace(Credentials.Name))
                MessageBox.Show("Введите имя");
            else if (String.IsNullOrWhiteSpace(Credentials.Surname))
                MessageBox.Show("Введите фамилию");
            else if (String.IsNullOrWhiteSpace(Credentials.Patronymic))
                MessageBox.Show("Введите отчество");
            else if (String.IsNullOrWhiteSpace(Credentials.FirstPassword))
                MessageBox.Show("Введите пароль");
            else if (String.IsNullOrWhiteSpace(Credentials.SecondPassword))
                MessageBox.Show("Подтвердите пароль");
            else if (String.IsNullOrWhiteSpace(Credentials.Email))
                MessageBox.Show("Введите электронную почту");
            else if(Credentials.FirstPassword != Credentials.SecondPassword)
                MessageBox.Show("Пароли должны совпадать");
            else
            {
                GetUsername();
                Registering?.Invoke(this, new RegisteringEventArgs(credentials.CurrentUsername, credentials.SecondPassword, credentials.Email));
            }
        }));//создать тут проверку на заполненность полей
        public CustomCommand ChangeRegistrationVisibility => changeRegistrationVisibility ?? (changeRegistrationVisibility = new CustomCommand(obj =>
        {
            if (RegistrationVisibility == Visibility.Visible)
                RegistrationVisibility = Visibility.Collapsed;
            else
                RegistrationVisibility = Visibility.Visible;
        }));

        public Visibility RegistrationVisibility
        {
            get => registrationVisibility;
            set
            {
                registrationVisibility = value;
                OnPropertyChanged();
            }
        }
        public CredentialData Credentials { get => credentials; set => credentials = value; }
        private void GetUsername()
        {
            Credentials.CurrentUsername = $"{Credentials.Surname}{Credentials.Name[0]}{Credentials.Patronymic[0]}";
        }
    }
}
