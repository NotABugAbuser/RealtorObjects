using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using RealtyModel.Model;

namespace RealtorObjects.Model
{
    /// <summary>
    /// Класс клиента в локальной сети.
    /// </summary>
    public class Client : INotifyPropertyChanged
    {
        Socket socket;
        Boolean isConnected;
        Dispatcher uiDispatcher;

        /// <summary>
        /// Свойство, которое равно true если клиент подключен к серверу, false если нет.
        /// </summary>
        public Boolean IsConnected
        {
            get => isConnected;
            private set
            {
                isConnected = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Коллекция сообщений лога событий данной клиента.
        /// </summary>
        public ObservableCollection<LogMessage> Log { get; private set; }
        /// <summary>
        /// Коллекция приходящих от сервера операций.
        /// </summary>
        public ObservableCollection<Operation> IncomingOperations { get; private set; }

        public Client(Dispatcher dispatcher)
        {
            uiDispatcher = dispatcher;
            Log = new ObservableCollection<LogMessage>();
            IncomingOperations = new ObservableCollection<Operation>();
        }

        /// <summary>
        /// Асинхронный метод подключения к серверу.
        /// Пока клиент подключен выполняется цикловый метод приёма операций.
        /// </summary>
        /// <param name="ipAddress">ipAddress - ip адрес сервера.</param>
        /// <returns></returns>
        public async Task ConnectAsync(IPAddress ipAddress)
        {
            await Task.Run(() =>
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, 8005);
                    socket.Connect(iPEndPoint);

                    IsConnected = true;
                    while (IsConnected) ReceiveMessage();
                }
                catch (Exception ex)
                {
                    UpdateLog(ex.Message);
                }
                finally
                {
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();
                    }
                    catch(Exception ex)
                    {
                        UpdateLog(ex.Message);
                    }
                }
            });
        }
        /// <summary>
        /// Метод отправки операций серверу. Операция передаётся в виде последовательности байт закодированной в UTF-8 json-строки.
        /// </summary>
        /// <param name="operation">operation - операция, которую необходимо отправить серверу.</param>
        public void SendMessage(Operation operation)
        {
            try
            {
                String json = JsonSerializer.Serialize<Operation>(operation);
                Byte[] data = Encoding.UTF8.GetBytes(json);
                socket.Send(data);
            }
            catch (Exception ex)
            {
                UpdateLog(ex.Message);
            }
        }
        /// <summary>
        /// Метод отключения от сервера.
        /// </summary>
        internal void Disconnect()
        {
            uiDispatcher.BeginInvoke(new Action(() =>
            {
                IsConnected = false;
            }));
        }
        /// <summary>
        /// Метод приём операции.
        /// </summary>
        private void ReceiveMessage()
        {
            Int32 byteCount = 0;
            Byte[] buffer = new Byte[256];
            StringBuilder message = new StringBuilder();

            do
            {
                byteCount = socket.Receive(buffer);
                message.Append(Encoding.UTF8.GetString(buffer), 0, byteCount);
            }
            while (socket.Available > 0);

            if (!String.IsNullOrWhiteSpace(message.ToString()))
            {
                Operation operation = JsonSerializer.Deserialize<Operation>(message.ToString());
                uiDispatcher.BeginInvoke(new Action(() =>
                {
                    IncomingOperations.Add(operation);
                }));
            }
        }
        /// <summary>
        /// Метод для добавления сообщений в лог событий (Log) при помощи диспетчера основного (UI) потока.
        /// </summary>
        /// <param name="message">message - текст сообщения. При вызове метода желательно указывать место вызова.</param>
        private void UpdateLog(String message)
        {
            uiDispatcher.BeginInvoke(new Action(() =>
            {
                Log.Add(new LogMessage(DateTime.Now.ToString("dd:MM:yy hh:mm"), message));
            }));
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
