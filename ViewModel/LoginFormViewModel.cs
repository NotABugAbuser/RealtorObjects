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
        private CredentialData credentialData = new CredentialData();
        private Visibility registrationVisibility = Visibility.Collapsed;
        public event RegisteringEventHandler Registering;

        public LoginFormViewModel() {
        }

        public CustomCommand Login => login ?? (login = new CustomCommand(obj => {
            Credential credential = new Credential() { Password = credentialData.CurrentPassword, Name = credentialData.CurrentUsername };
            if (Client.Login(credential)) {
                new MainWindowV2() { DataContext = new MainWindowViewModel(credential) }.Show();
                (obj as LoginForm).Close();
            }
        }));
        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj => {
            Application.Current.Shutdown();
        }));
        public CustomCommand SendPassword => sendPassword ?? (sendPassword = new CustomCommand(obj => {

        }));
        public CustomCommand CreateNewUser => createNewUser ?? (createNewUser = new CustomCommand(obj => {
            if (String.IsNullOrWhiteSpace(CredentialData.Name))
                MessageBox.Show("Введите имя");
            else if (String.IsNullOrWhiteSpace(CredentialData.Surname))
                MessageBox.Show("Введите фамилию");
            else if (String.IsNullOrWhiteSpace(CredentialData.Patronymic))
                MessageBox.Show("Введите отчество");
            else if (String.IsNullOrWhiteSpace(CredentialData.FirstPassword))
                MessageBox.Show("Введите пароль");
            else if (String.IsNullOrWhiteSpace(CredentialData.SecondPassword))
                MessageBox.Show("Подтвердите пароль");
            else if (String.IsNullOrWhiteSpace(CredentialData.Email))
                MessageBox.Show("Введите электронную почту");
            else if (CredentialData.FirstPassword != CredentialData.SecondPassword)
                MessageBox.Show("Пароли должны совпадать");
            else {
                GetUsername();
                Registering?.Invoke(this, new RegisteringEventArgs(credentialData.CurrentUsername, credentialData.SecondPassword, credentialData.Email));
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
        public CredentialData CredentialData {
            get => credentialData; 
            set => credentialData = value;
        }
        private void GetUsername() {
            CredentialData.CurrentUsername = $"{CredentialData.Surname}{CredentialData.Name[0]}{CredentialData.Patronymic[0]}";
        }
    }
}
