using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    class LoginFormViewModel : BaseViewModel
    {
        CustomCommand closeApp;
        CustomCommand submit;
        string currentPassword = "";
        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj => {
             Application.Current.Shutdown();
        }));
        public CustomCommand Submit => submit ?? (submit = new CustomCommand(obj => {
            MessageBox.Show(CurrentPassword);
        }));
        public string CurrentPassword {
            get => currentPassword;
            set {
                currentPassword = value;
                OnPropertyChanged();
            }
        }
    }
}
