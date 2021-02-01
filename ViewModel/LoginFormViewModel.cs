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

namespace RealtorObjects.ViewModel
{
    class LoginFormViewModel : BaseViewModel
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

        private Boolean isLoggedIn = false;
        private Client client = new Client(Dispatcher.CurrentDispatcher);

        public LoginFormViewModel()
        {
            //Task connectTask = client.ConnectAsync(IPAddress.Loopback);
            //Task checkIncomingOps = CheckIncomingOps();
        }

        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj =>
        {
            Application.Current.Shutdown();
        }));
        public CustomCommand SendPassword => sendPassword ?? (sendPassword = new CustomCommand(obj =>
        { 

        }));
        public CustomCommand ChangeRegistrationVisibility => changeRegistrationVisibility ?? (changeRegistrationVisibility = new CustomCommand(obj =>
        { // меняет форму
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
        { // отсылает запрос на создание пользователя в базу
            CurrentLogin = Surname + Name.Remove(1, Name.Length - 1) + Patronymic.Remove(1, Patronymic.Length - 1);
            Operation register = new Operation()
            {
                Name = CurrentLogin,
                OperationParameters = new OperationParameters()
                {
                    Direction = OperationDirection.Identity,
                    Type = OperationType.Register
                },
                Data = CurrentPassword
            };
            client.SendMessage(register);
        }, obj =>
        { // проверяет поля формы на заполненность
            return !(String.IsNullOrEmpty(Name)
            && String.IsNullOrEmpty(Surname)
            && String.IsNullOrEmpty(Patronymic)
            && String.IsNullOrEmpty(Email)
            && String.IsNullOrEmpty(FirstPassword)
            && String.IsNullOrEmpty(SecondPassword));
        }));
        public CustomCommand Login => login ?? (login = new CustomCommand(obj =>
        { 

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

        /// <summary>
        /// Асинхронный метод с циклом проверки наличия входящих(с сервера) операций.
        /// P.S. Этот метод в конечном варианте будет немного другим.
        /// P.S.S. Этот метод как и Handle должен быть в главном MainWindowVM, но на период тестов пока тут.
        /// </summary>
        /// <returns></returns>
        private async Task CheckIncomingOps()
        {
            await Task.Run(new Action(() =>
            {
                while (true)
                {
                    Queue<Operation> incomingOps = client.IncomingOperations;
                    
                    while (incomingOps.Count > 0)
                    {
                        //Если первая в очереди операция направлена на идентификацию то достать из очереди и обработать
                        if (incomingOps.Peek().OperationParameters.Direction == OperationDirection.Identity)
                            Handle(incomingOps.Dequeue());
                        
                    }
                }
            }));
        }
        /// <summary>
        /// Метод обработки входящих(с сервера) операций.
        /// </summary>
        /// <param name="operation"></param>
        private void Handle(Operation operation)
        {
            //Если авторизация успешна
            if (operation.OperationParameters.Type == OperationType.Login && operation.IsSuccessfully)
            {
                //Должно выпасть сообщение об успешном логине
                //Так же меняется статус клиента(имеется в виду самого приложения, а не экземпляра класса) на авторизован
                isLoggedIn = true;
            }
            //Если регистрация успешна
            else if (operation.OperationParameters.Type == OperationType.Register && operation.IsSuccessfully)
            {
                //должно выпасть сообщение об успешной регистрации
            }
        }
    }
}
