using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using RealtorObjects.Model;
using RealtyModel.Model;

namespace RealtorObjects.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        //Нужен ли для MainWindow доступ в сеть?
        //Необходимо временно добавить лог событий
        //Как можно скорее добавить индикатор подключения, хотябы на период тестирования

        Boolean isLoggedIn = false;
        
        public Client Client { get; }
        public Boolean IsLoggedIn
        {
            get => isLoggedIn;
            private set
            {
                isLoggedIn = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<LogMessage> Log { get; private set; }

        public BaseViewModel()
        {
            Log = new ObservableCollection<LogMessage>();
            
            Client = new Client(Dispatcher.CurrentDispatcher);
            Client.Log.CollectionChanged += (sender, e) => UpdateLog(e.NewItems);
            Client.IncomingOperations.CollectionChanged += (sender, e) => HandleOperation(e.NewItems);

            ConnectAsync();
        }

        public async void ConnectAsync()
        {
            Task connection = Client.ConnectAsync(IPAddress.Loopback);
            try
            {
                await connection;
            }                    
            catch(Exception ex)
            {
                UpdateLog("connection is failed. " + ex.Message);
            }
        }
        protected void UpdateLog(String message)
        {
            
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                Log.Add(new LogMessage(DateTime.Now.ToString("dd:MM:yy hh:mm"), message));
            }));
        }
        protected void UpdateLog(IList messages)
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
        protected void HandleOperation(IList messages)
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
