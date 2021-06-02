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
        private CredentialData credentialData = new CredentialData();

        public LoginFormViewModel() {
        }

        public CustomCommand Login => login ?? (login = new CustomCommand(obj => {
            Credential credential = new Credential() { Password = credentialData.CurrentPassword, Name = credentialData.CurrentUsername };
            if (Client.Login(credential)) {
                ((App)Application.Current).AgentName = credential.Name;
                new MainWindowV2(new MainWindowViewModel()).Show();
                (obj as LoginForm).Close();
            }
        }));
        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj => {
            Application.Current.Shutdown();
        }));
        public CustomCommand SendPassword => sendPassword ?? (sendPassword = new CustomCommand(obj => {

        }));
        public CredentialData CredentialData {
            get => credentialData; 
            set => credentialData = value;
        }
    }
}
