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
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;
using System.Threading;

namespace RealtorObjects.Model
{
    /// <summary>
    /// Класс клиента в локальной сети.
    /// </summary>
    public class Client : INotifyPropertyChanged
    {
        public List<String> Vs { get; set; }

        Socket socket = null;
        Boolean isConnected = false;
        Boolean isTryingToConnect = false;
        Dispatcher uiDispatcher = null;

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
        public Boolean IsTryingToConnect
        {
            get => isTryingToConnect;
            private set
            {
                isTryingToConnect = value;
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
        public Queue<Operation> IncomingOperations { get; private set; }

        public Client(Dispatcher dispatcher)
        {
            Vs = new List<string>();
            uiDispatcher = dispatcher;
            Log = new ObservableCollection<LogMessage>();
            IncomingOperations = new Queue<Operation>();
        }

        /// <summary>
        /// Асинхронный метод подключения к серверу.
        /// Пока клиент подключен выполняется цикловый метод приёма операций.
        /// </summary>
        /// <param name="ipAddress">ipAddress - ip адрес сервера.</param>
        /// <returns></returns>
        public async void ConnectAsync()
        {
            await Task.Run(() =>
            {
                IsTryingToConnect = true;
                IPAddress ipAddress = FindServerIPAddress();
                if (ipAddress != null)
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    try
                    {
                        IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, 8005);
                        socket.Connect(iPEndPoint);
                        IsConnected = true;
                        IsTryingToConnect = false;
                        while (IsConnected) ReceiveMessage();
                    }
                    catch (Exception ex)
                    {
                        UpdateLog(ex.Message);
                    }
                    finally
                    {
                        IsConnected = false;
                        try
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                        }
                        catch (Exception ex)
                        {
                            UpdateLog(ex.Message);
                        }
                    }
                }
            });
        }
        /// <summary>
        /// Метод поиска адреса сервера при помощи широковещательной UDP рассылки.
        /// </summary>
        /// <returns></returns>
        private IPAddress FindServerIPAddress()
        {
            IPAddress serverIP = null;
            try
            {
                Socket socket = null;

                IPAddress[] iPAddresses = GetLocalIPv4Addresses();
                do
                {
                    foreach (IPAddress iP in iPAddresses)
                    {
                        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);
                        socket.EnableBroadcast = true;
                        socket.ReceiveTimeout = 1000;
                        socket.Bind(new IPEndPoint(iP, 8080));
                        for (Int32 attempts = 1; attempts < 11 && serverIP == null; attempts++)
                        {
                            socket.SendTo(new byte[] { 0x10 }, new IPEndPoint(IPAddress.Broadcast, 8080));
                            Int32 byteCount = 0;
                            byte[] buffer = new byte[socket.ReceiveBufferSize];
                            EndPoint endPoint = new IPEndPoint(IPAddress.None, 0);
                            if (socket.Poll(1000000, SelectMode.SelectRead))
                            {
                                byteCount = socket.ReceiveFrom(buffer, ref endPoint);
                                if (byteCount == 1 && buffer[0] == 0x20)
                                    serverIP = (endPoint as IPEndPoint).Address;
                            }
                            Thread.Sleep(200);
                        }
                        if (serverIP != null) break;
                        socket.Dispose();
                        socket.Close();
                    }
                }
                while (serverIP == null && IsTryingToConnect);
                return serverIP;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"(FindServer) {ex.Message} {ex.StackTrace}");
                return null;
            }
            finally
            {
                if (serverIP == null)
                    Console.WriteLine("server is unreachable");
                else Console.WriteLine($"server has found on {serverIP}");
            }
        }
        /// <summary>
        /// Метод возвращает массив IPv4 адресов рабочих адаптеров данного ПК, которые используют сетевой протокол IEEE 802.3(Ethernet) или IEEE 802.11(Wi-Fi).
        /// </summary>
        /// <returns></returns>
        private IPAddress[] GetLocalIPv4Addresses()
        {
            NetworkInterface[] netInterfaces = NetworkInterface.GetAllNetworkInterfaces().Where(inf =>
                inf.OperationalStatus == OperationalStatus.Up
                && (inf.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                || inf.NetworkInterfaceType == NetworkInterfaceType.Wireless80211
                )).ToArray();

            List<IPAddress> addresses = new List<IPAddress>();

            foreach (NetworkInterface netInterface in netInterfaces)
            {
                foreach (UnicastIPAddressInformation unicastIP in netInterface.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIP.Address.AddressFamily == AddressFamily.InterNetwork)
                        addresses.Add(unicastIP.Address);
                }
            }
            return addresses.ToArray();
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
                IsTryingToConnect = false;
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
                IncomingOperations.Enqueue(operation);
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
