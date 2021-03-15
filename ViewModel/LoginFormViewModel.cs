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

        private Client client = new Client(Dispatcher.CurrentDispatcher);

        public LoginFormViewModel() {

        }

        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj => {
            Application.Current.Shutdown();
        }));
        public CustomCommand SendPassword => sendPassword ?? (sendPassword = new CustomCommand(obj => {

        }));
        public CustomCommand ChangeRegistrationVisibility => changeRegistrationVisibility ?? (changeRegistrationVisibility = new CustomCommand(obj => {            
            if (RegistrationVisibility == Visibility.Visible) {
                RegistrationVisibility = Visibility.Collapsed;
            } else {
                RegistrationVisibility = Visibility.Visible;
            }
        }));
        public CustomCommand CreateNewUser => createNewUser ?? (createNewUser = new CustomCommand(obj => {
            CurrentLogin = $"{Surname}{Name[0]}{Patronymic[0]}";
            
        }, obj => { // проверяет поля формы на заполненность
            return !(String.IsNullOrEmpty(Name)
            && String.IsNullOrEmpty(Surname)
            && String.IsNullOrEmpty(Patronymic)
            && String.IsNullOrEmpty(Email)
            && String.IsNullOrEmpty(FirstPassword)
            && String.IsNullOrEmpty(SecondPassword));
        }));
        public CustomCommand Login => login ?? (login = new CustomCommand(obj => {
            var isLoginSuccessful = true;
            /*
            * 
            *  твой код обращения к серверу и получения ответа здесь 
            *
            */
            if (isLoginSuccessful) {
                Logged?.Invoke(this, new LoggedEventArgs(obj));
            }
        }));

        public string CurrentLogin {
            get => currentLogin;
            set {
                currentLogin = value;
                OnPropertyChanged();
            }
        }
        public string CurrentPassword {
            get => currentPassword;
            set {
                currentPassword = value;
                OnPropertyChanged();
            }
        }
        public Visibility RegistrationVisibility {
            get => registrationVisibility;
            set {
                registrationVisibility = value;
                OnPropertyChanged();
            }
        }
        public string Name {
            get => name;
            set {
                name = value;
                OnPropertyChanged();
            }
        }
        public string Surname {
            get => surname;
            set {
                surname = value;
                OnPropertyChanged();
            }
        }
        public string Patronymic {
            get => patronymic;
            set {
                patronymic = value;
                OnPropertyChanged();
            }
        }
        public string Email {
            get => email;
            set {
                email = value;
                OnPropertyChanged();
            }
        }
        public string FirstPassword {
            get => firstPassword;
            set {
                firstPassword = value;
                OnPropertyChanged();
            }
        }
        public string SecondPassword {
            get => secondPassword;
            set {
                secondPassword = value;
                OnPropertyChanged();
            }
        }

        public Client Client { get => client; set => client = value; }

        public event LoggedEventHandler Logged;
    }
}
