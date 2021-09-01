using RealtorObjects.Model;
using System;
using System.Windows;
using RealtyModel.Model;
using RealtorObjects.View;
using System.Threading.Tasks;
using System.Diagnostics;
using RealtyModel.Model.Tools;

namespace RealtorObjects.ViewModel
{
    public class LoginFormVM : BaseVM
    {
        private CustomCommand login;
        private CustomCommand closeApp;
        private Credential credential = new Credential();
        public LoginFormVM() {
        }
        public CustomCommand Login => login ?? (login = new CustomCommand(obj => {
            //if (Client.CanConnect()) {
            //    Tuple<bool, int> pair = Client.Login(credential);
            //    if (pair.Item1) {
            //        Client.Name = credential.Name;
            //        MainWindowVM mainVM = new MainWindowVM(credential.Name, pair.Item2);
            //        Debug.WriteLine($"{pair.Item2} {credential.Name}");
            //        new MainWindowV3(mainVM).Show();
            //        Credential = new Credential();
            //        (obj as Window).Close();
            //    }
            //}
                    new MainWindowV3(new MainWindowVM()).Show();
        }));
        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj => {
            Application.Current.Shutdown();
        }));
        public Credential Credential {
            get => credential;
            set => credential = value;
        }
    }
}
