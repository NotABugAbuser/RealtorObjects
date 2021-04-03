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

namespace RealtorObjects.ViewModel
{
    public class LoginFormViewModel : BaseViewModel
    {
        private CustomCommand closeApp;
        private CustomCommand sendPassword;
        private CustomCommand changeRegistrationVisibility;
        private CustomCommand createNewUser;
        private CustomCommand login;
        private CredentialData credentials = new CredentialData();
        private Visibility registrationVisibility = Visibility.Collapsed;
        

        public LoginFormViewModel() {
            this.Client = ((App)Application.Current).Client;
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
            Operation operation = new Operation(Credentials.CurrentUsername, Credentials.SecondPassword, OperationDirection.Identity, OperationType.Register);
            Client.SendMessage(operation);

            for (byte attempts = 0; attempts <= 30; attempts++) {
                if (Client.IncomingOperations.Count > 0) {
                    Operation incomingOperation = Client.IncomingOperations.Dequeue();
                    bool isRegistrationSuccessful = incomingOperation.OperationParameters.Direction == OperationDirection.Identity
                        && incomingOperation.OperationParameters.Type == OperationType.Register
                        && incomingOperation.Name == Credentials.CurrentUsername
                        && incomingOperation.IsSuccessfully;
                    if (isRegistrationSuccessful) {
                        MessageBox.Show("Регистрация прошла успешно");
                        RegistrationVisibility = Visibility.Collapsed;
                        break;
                    }
                }
                if (attempts == 30) MessageBox.Show("Что-то пошло не так");
                Thread.Sleep(100);
            }
        }));//создать тут проверку на заполненность полей

        public CustomCommand Login => login ?? (login = new CustomCommand(obj => {
            Operation operation = new Operation(Credentials.CurrentUsername, Credentials.CurrentPassword, OperationDirection.Identity, OperationType.Login);
            Client.SendMessage(operation);

            for (byte attempts = 0; attempts <= 30; attempts++) {
                if (Client.IncomingOperations.Count > 0) {
                    Operation incomingOperation = Client.IncomingOperations.Dequeue();
                    bool isRightAgent = incomingOperation.OperationParameters.Direction == OperationDirection.Identity
                        && incomingOperation.OperationParameters.Type == OperationType.Login
                        && incomingOperation.Name == Credentials.CurrentUsername;
                    if (isRightAgent) {
                        if (incomingOperation.IsSuccessfully) {
                            Client.CurrentAgent = Credentials.CurrentUsername;
                            Logged?.Invoke(this, new LoggedEventArgs(obj));
                            break;
                        } else {
                            MessageBox.Show("Введены неверные данные");
                            break;
                        }
                    }
                    if (attempts == 30) MessageBox.Show("Связь с сервером идентификации прервана");
                }
                Thread.Sleep(100);
            }
        }));
        public Visibility RegistrationVisibility {
            get => registrationVisibility;
            set {
                registrationVisibility = value;
                OnPropertyChanged();
            }
        }

        public CredentialData Credentials { get => credentials; set => credentials = value; }

        public event LoggedEventHandler Logged;
    }
}
