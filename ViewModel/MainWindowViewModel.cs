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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using RealtyModel.Service;
using RealtyModel.Model;
using RealtorObjects.Model;
using System.Net;
using System.Collections;

namespace RealtorObjects.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private Boolean isLoggedIn = false;
        private Client сlient = new Client(Dispatcher.CurrentDispatcher);
        private CustomCommand testCommand;
        private BaseViewModel workAreaViewModel;
        private CustomCommand secondTestCommand;
        private CustomCommand updateWorkAreaViewModel;
        private string header = "Главная";
        private FontAwesomeIcon currentIcon = FontAwesomeIcon.Home;
        private BaseViewModel[] viewModels;
        private FontAwesomeIcon[] icons = new FontAwesomeIcon[5] {
            FontAwesomeIcon.Home,
            FontAwesomeIcon.Phone,
            FontAwesomeIcon.BarChart,
            FontAwesomeIcon.List,
            FontAwesomeIcon.AddressBook,
        };
        ObservableCollection<bool> toggledButtons = new ObservableCollection<bool> {
            true,
            false,
            false,
            false,
            false
        };
        private string[] headers = new string[5]{
            "Главная",
            "Номера телефонов",
            "Статистика",
            "Объекты",
            "Клиенты",
        };
        private string currentTime;

        public MainWindowViewModel()
        {
            string dayOfWeek = new CultureInfo("ru-RU").DateTimeFormat.GetShortestDayName(DateTime.Now.DayOfWeek);
            CurrentTime = $"{DateTime.Now:HH : mm} {dayOfWeek}";
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    CurrentTime = $"{DateTime.Now:HH:mm} {dayOfWeek}";
                }
            });
            
            Log = new ObservableCollection<LogMessage>();
            WorkAreaViewModel = new RealtorObjectsViewModel(Log);
            viewModels = new BaseViewModel[] {
            new HomeViewModel(Log),
            new PhoneNumbersViewModel(Log),
            new StatisticsViewModel(Log),
            new RealtorObjectsViewModel(Log),
            new CustomersViewModel(Log)
            };
            
            сlient.IncomingOperations.CollectionChanged += (sender, e) => HandleOperation(e.NewItems);
            сlient.Log.CollectionChanged += (sender, e) => UpdateLog(e.NewItems);
        }

        public CustomCommand UpdateWorkAreaViewModel => updateWorkAreaViewModel ?? (updateWorkAreaViewModel = new CustomCommand(obj =>
        {
            byte index = Convert.ToByte(obj);
            WorkAreaViewModel = viewModels[index];
            Header = headers[index];
            CurrentIcon = icons[index];
            for (byte i = 0; i < ToggledButtons.Count; i++)
            {
                ToggledButtons[i] = false;
            }
            ToggledButtons[index] = true;
        }));
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj =>
        {
            var flatWindow = new FlatForm();
            flatWindow.Show();
        }));
        public CustomCommand SecondTestCommand => secondTestCommand ?? (secondTestCommand = new CustomCommand(obj =>
        {
        }));
        public BaseViewModel WorkAreaViewModel
        {
            get => workAreaViewModel;
            set
            {
                workAreaViewModel = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<bool> ToggledButtons => toggledButtons;
        /// <summary>
        /// Коллекция сообщений лога событий приложения.
        /// </summary>
        public ObservableCollection<LogMessage> Log { get; private set; }
        /// <summary>
        /// Свойство, которое равно true если клиент залогинен, false если нет.
        /// </summary>
        public Boolean IsLoggedIn
        {
            get => isLoggedIn;
            private set
            {
                isLoggedIn = value;
                OnPropertyChanged();
            }
        }

        public string CurrentTime
        {
            get => currentTime;
            set
            {
                currentTime = value;
                OnPropertyChanged();
            }
        }

        public FontAwesomeIcon CurrentIcon
        {
            get => currentIcon;
            set
            {
                currentIcon = value;
                OnPropertyChanged();
            }
        }

        public string Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// НЕ ОКОНЧАТЕЛЬНАЯ ВЕРСИЯ!
        /// Метод асинхронного подключения к серверу.
        /// Возварщает экземпляр Task для отслеживания состояния задачи.
        /// </summary>
        private async void ConnectAsync()
        {
            Task connection = сlient.ConnectAsync(IPAddress.Loopback);
            try
            {
                await connection;
            }
            catch (Exception ex)
            {
                UpdateLog("connection is failed. " + ex.Message);
            }
        }
        /// <summary>
        /// Метод для добавления сообщений в лог событий (Log) при помощи диспетчера основного (UI) потока.
        /// </summary>
        /// <param name="message">message - текст сообщения. При вызове метода желательно указывать место вызова.</param>
        private void UpdateLog(String message)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                Log.Add(new LogMessage(DateTime.Now.ToString("dd:MM:yy hh:mm"), message));
            }));
        }
        /// <summary>
        /// Метод для добавления сообщений в лог событий (Log) при помощи диспетчера основного (UI) потока.
        /// Используется для подписи на событие CollectionCahnged других логов событий.
        /// </summary>
        /// <param name="messages">messages - список новых сообщений отслеживаемого лога.</param>
        private void UpdateLog(IList messages)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                foreach (LogMessage message in messages)
                {
                    try
                    {
                        Log.Add(message);
                    }
                    catch (Exception ex)
                    {
                        UpdateLog(ex.Message);
                    }
                }
            }));
        }
        /// <summary>
        /// НЕ ОКОНЧАТЕЛЬНАЯ ВЕРСИЯ!
        /// Метод для обработки приходящих от сервера операций.
        /// Используется для подписи на событие CollectionCahnged коллекции выходящих операций клиента.
        /// </summary>
        /// <param name="messages">messages - список новых операций.</param>
        private void HandleOperation(IList messages)
        {
            foreach (Operation operation in messages)
            {
                try
                {
                    if (operation.OperationType == OperationType.Register)
                    {
                        if (operation.IsSuccessfully)
                        {
                            UpdateLog("Registration is successfull");
                        }
                        else UpdateLog("Registration is not successfull");
                    }
                    else if (operation.OperationType == OperationType.Login)
                    {
                        if (operation.IsSuccessfully)
                        {
                            IsLoggedIn = true;
                            UpdateLog("Log in is successfull");
                        }
                        else UpdateLog("Log in is not successfull");
                    }
                }
                catch (Exception ex)
                {
                    UpdateLog(ex.Message);
                }
            }
        }
        
        public void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
