using RealtorObjects.Model;
using RealtyModel.Service;
using System;
using System.Windows;
using RealtyModel.Model;
using RealtorObjects.View;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RealtorObjects.ViewModel
{
    public class LoginFormViewModel : BaseViewModel {
        private CustomCommand login;
        private CustomCommand closeApp;
        private CredentialData credentialData = new CredentialData();
        public LoginFormViewModel() {
            if (Debugger.IsAttached) {
                CredentialData.CurrentUsername = "Админ";
                CredentialData.CurrentPassword = "csharprulit";
            }
        }
        public CustomCommand Login => login ?? (login = new CustomCommand(obj => {
            Credential credential = new Credential() { Password = credentialData.CurrentPassword, Name = credentialData.CurrentUsername };
            if (Client.Login(credential)) {
                ((App)Application.Current).AgentName = credential.Name;
                Client.Name = credential.Name;
                MainWindowViewModel mainVM = new MainWindowViewModel();
                new MainWindowV3(mainVM).Show();
                (obj as Window).Close();
            }
        }));
        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj => {
            Application.Current.Shutdown();
        }));
        public CredentialData CredentialData
        {
            get => credentialData;
            set => credentialData = value;
        }
    }
}
