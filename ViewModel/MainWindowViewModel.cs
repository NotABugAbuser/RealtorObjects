using FontAwesome.WPF;
using RealtorObjects.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using RealtyModel.Model;
using RealtyModel.Service;
using System.Windows.Threading;
using System.Net;
using RealtorObjects.Model;

namespace RealtorObjects.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private BaseViewModel workAreaViewModel;
        private CustomCommand updateWorkAreaViewModel;
        private CustomCommand closeApp;
        private string header = "Главная";
        private string currentTime;
        private FontAwesomeIcon currentIcon = FontAwesomeIcon.Home;
        private LocationOptions locationOptions = new LocationOptions();
        private BaseViewModel[] viewModels = new BaseViewModel[] {
            new LoginFormViewModel(),
            new HomeViewModel(),
            new CustomersViewModel(),
            new LocationsViewModel()
        };
        private FontAwesomeIcon[] icons = new FontAwesomeIcon[5] {
            FontAwesomeIcon.Home,
            FontAwesomeIcon.Phone,
            FontAwesomeIcon.BarChart,
            FontAwesomeIcon.List,
            FontAwesomeIcon.AddressBook,
        };
        readonly ObservableCollection<bool> toggledButtons = new ObservableCollection<bool> {
            true,
            false,
            false,
            false,
            false
        };
        private readonly string[] headers = new string[2]{
            "Главная",
            "Клиенты",
        };
        private Boolean isLoggedIn = false;
        private Client client = new Client(Dispatcher.CurrentDispatcher);
        #region TestMethods
        private CustomCommand testCommand;
        private CustomCommand secondTestCommand;
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj => {
            var flatWindow = new FlatFormV2();
            flatWindow.Show();
        }));
        public CustomCommand SecondTestCommand => secondTestCommand ?? (secondTestCommand = new CustomCommand(obj => {
        }));
        #endregion
        public List<LogMessage> Log {
            get; private set;
        }
        public Boolean IsLoggedIn {
            get => isLoggedIn;
            private set {
                isLoggedIn = value;
                OnPropertyChanged();
            }
        }
       
        /// <summary>
        /// Метод для добавления сообщений в лог событий (Log) при помощи диспетчера основного (UI) потока.
        /// </summary>
        /// <param name="message">message - текст сообщения. При вызове метода желательно указывать место вызова.</param>
        private void UpdateLog(String message) {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => {
                Log.Add(new LogMessage(DateTime.Now.ToString("dd:MM:yy hh:mm"), message));
            }));
        }
        #region HandlingMethods
        private void HandleOperation(Operation operation) {
            try {
                if (operation.OperationParameters.Direction == OperationDirection.Identity) {
                    if (operation.IsSuccessfully) {
                        HandleSuccessfullIdentity(operation);
                    } else {
                        HandleUnsuccessfullIdentity(operation);
                    }
                } else if (operation.OperationParameters.Direction == OperationDirection.Realty) {
                    if (operation.IsSuccessfully) {
                        HandleSuccessfullRealty(operation);
                    } else {
                        HandleSuccessfullRealty(operation);
                    }
                }
            } catch (Exception ex) {
                UpdateLog(ex.Message);
            }
        }
        private void HandleSuccessfullIdentity(Operation operation) {
            switch (operation.OperationParameters.Type) {
                case OperationType.Login: {
                        //LoginVM.IsLoggedIn = true;
                        break;
                    }
                case OperationType.Logout: {
                        //LoginVM.IsLoggedIn = false;
                        break;
                    }
                case OperationType.Register: {
                        //LoginVM.Message = "Регистрация была успешной";
                        break;
                    }
                case OperationType.ToFire: {
                        //LoginVM.Message = "Удаление учётки было успешным";
                        //Выбросить в окно регистрации
                        break;
                    }
            }
        }
        private void HandleSuccessfullRealty(Operation operation) {
            switch (operation.OperationParameters.Type) {
                case OperationType.Add: {
                        //HomeVM.LocationOptions - добавить новые объекты в списки
                        //LocationVM.LocationOptions - добавить новые объекты в списки
                        break;
                    }
                case OperationType.Change: {
                        //HomeVM.LocationOptions - обновить по Id необходимые объекты в списках
                        //LocationVM.LocationOptions - обновить по Id необходимые объекты в списках
                        break;
                    }
                case OperationType.Remove: {
                        //HomeVM.LocationOptions - удалить по Id необходимые объекты из списков
                        //LocationVM.LocationOptions - удалить по Id необходимые объекты из списков
                        break;
                    }
                case OperationType.Update: {
                        //HomeVM.LocationOptions - получение всех списков от сервера
                        //LocationVM.LocationOptions - получение всех списков от сервера
                        break;
                    }
            }
        }
        private void HandleUnsuccessfullIdentity(Operation operation) {
            switch (operation.OperationParameters.Type) {
                case OperationType.Login: {
                        //LoginVM.Message = "Логин не был успешным";
                        break;
                    }
                case OperationType.Logout: {
                        //LoginVM.Message = "Логаут не был успешным";
                        break;
                    }
                case OperationType.Register: {
                        //LoginVM.Message = "Регистрация не была успешной";
                        break;
                    }
                case OperationType.ToFire: {
                        //LoginVM.Message = "Удаление учётки не было успешным";
                        break;
                    }
            }
        }
        private void HandleUnsuccessfullReality(Operation operation) {
            switch (operation.OperationParameters.Type) {
                case OperationType.Add: {
                        //Сообщить о неуспешности
                        break;
                    }
                case OperationType.Change: {
                        //Сообщить о неуспешности
                        break;
                    }
                case OperationType.Remove: {
                        //Сообщить о неуспешности
                        break;
                    }
                case OperationType.Update: {
                        //Сообщить о неуспешности
                        break;
                    }
            }
        }
        #endregion

        public MainWindowViewModel() {
            new LoginForm() { DataContext = viewModels[0] }.Show();
            WorkAreaViewModel = viewModels[1];
            ((LoginFormViewModel)viewModels[0]).TryRegister += (operation) => client.SendMessage(operation);
            Connect();


            string dayOfWeek = new CultureInfo("ru-RU").DateTimeFormat.GetShortestDayName(DateTime.Now.DayOfWeek);
            CurrentTime = $"{DateTime.Now:HH:mm} {dayOfWeek}";
            Task.Factory.StartNew(() => {
                while (true) {
                    Thread.Sleep(5000);
                    CurrentTime = $"{DateTime.Now:HH:mm} {dayOfWeek}";
                }
            });
        }
        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj => {
            Application.Current.Shutdown();
        }));
        public CustomCommand UpdateWorkAreaViewModel => updateWorkAreaViewModel ?? (updateWorkAreaViewModel = new CustomCommand(obj => {
            byte index = Convert.ToByte(obj);
            WorkAreaViewModel = viewModels[index];
            Header = headers[index];
            CurrentIcon = icons[index];
            for (byte i = 0; i < ToggledButtons.Count; i++) {
                ToggledButtons[i] = false;
            }
            ToggledButtons[index] = true;
        }));
        public BaseViewModel WorkAreaViewModel {
            get => workAreaViewModel;
            set {
                workAreaViewModel = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<bool> ToggledButtons => toggledButtons;
        public string CurrentTime {
            get => currentTime;
            set {
                currentTime = value;
                OnPropertyChanged();
            }
        }
        public FontAwesomeIcon CurrentIcon {
            get => currentIcon;
            set {
                currentIcon = value;
                OnPropertyChanged();
            }
        }
        public string Header {
            get => header;
            set {
                header = value;
                OnPropertyChanged();
            }
        }
        public LocationOptions LocationOptions {
            get => locationOptions;
            set {
                locationOptions = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// НЕ ОКОНЧАТЕЛЬНАЯ ВЕРСИЯ!
        /// Метод асинхронного подключения к серверу.
        /// Возварщает экземпляр Task для отслеживания состояния задачи.
        /// </summary>
        private void Connect() {
            try {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => client.ConnectAsync(IPAddress.Parse("192.168.1.107"))));
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => AwaitOperationAsync()));
            

            }
            catch (Exception ex) {
                UpdateLog("connection is failed. " + ex.Message);
            }
        }
        private async void AwaitOperationAsync() {
            await Task.Run(() => {
                while (true) {
                    while (client.IncomingOperations.Count > 0) {
                        HandleOperation(client.IncomingOperations.Dequeue());
                    }
                }
            });
        }
        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
//добавить поле для текстблока, который будет отображать успешность логина в HandleOperation