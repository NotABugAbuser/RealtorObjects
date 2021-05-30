using RealtorObjects.Model;
using RealtyModel.Service;
using System;
using System.Windows;
using RealtyModel.Events.Identity;
using RealtyModel.Model;
using RealtorObjects.View;

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
        public event RegisteringEventHandler Registering;

        public LoginFormViewModel() {
        }

        public CustomCommand Login => login ?? (login = new CustomCommand(obj => {
            Credential credential = new Credential() { Password = credentials.CurrentPassword, Name = credentials.CurrentUsername };
            if (Client.Login(credential)) {
                new MainWindowV2() { DataContext = new MainWindowViewModel(credential) }.Show();
            }
        }));
        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj => {
            Application.Current.Shutdown();
        }));
        public CustomCommand SendPassword => sendPassword ?? (sendPassword = new CustomCommand(obj => {

        }));
        public CustomCommand CreateNewUser => createNewUser ?? (createNewUser = new CustomCommand(obj => {
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
            else if (Credentials.FirstPassword != Credentials.SecondPassword)
                MessageBox.Show("Пароли должны совпадать");
            else {
                GetUsername();
                Registering?.Invoke(this, new RegisteringEventArgs(credentials.CurrentUsername, credentials.SecondPassword, credentials.Email));
            }
        }));//создать тут проверку на заполненность полей
        public CustomCommand ChangeRegistrationVisibility => changeRegistrationVisibility ?? (changeRegistrationVisibility = new CustomCommand(obj => {
            if (RegistrationVisibility == Visibility.Visible)
                RegistrationVisibility = Visibility.Collapsed;
            else
                RegistrationVisibility = Visibility.Visible;
        }));

        public Visibility RegistrationVisibility {
            get => registrationVisibility;
            set {
                registrationVisibility = value;
                OnPropertyChanged();
            }
        }
        public CredentialData Credentials {
            get => credentials; set => credentials = value;
        }
        private void GetUsername() {
            Credentials.CurrentUsername = $"{Credentials.Surname}{Credentials.Name[0]}{Credentials.Patronymic[0]}";
        }
    }
}
