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
using System.Windows;
using System.Diagnostics;

namespace RealtorObjects.Model
{
    /// <summary>
    /// Класс клиента в локальной сети.
    /// </summary>
    public class Client : INotifyPropertyChanged
    {
        private String currentAgent = "";
        public string CurrentAgent
        {
            get => currentAgent;
            set => currentAgent = value;
        }

        private Boolean isConnected = false;
        private Boolean isTryingToConnect = false;
        private Socket socket = null;
        private Dispatcher uiDispatcher = null;
        public delegate void ConnectedEventHandler();
        public delegate void LostConnectionEventHandler();
        public event ConnectedEventHandler Connected;
        public event LostConnectionEventHandler LostConnection;
        public event PropertyChangedEventHandler PropertyChanged;

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
        /// Коллекция приходящих от сервера операций.
        /// </summary>
        public Queue<Operation> IncomingOperations
        {
            get; private set;
        }

        public Client()
        {

        }
        public Client(Dispatcher dispatcher)
        {
            uiDispatcher = dispatcher;
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
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    if (ipAddress != null)
                    {
                        IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, 8005);
                        socket.Connect(iPEndPoint);
                        IsTryingToConnect = false;
                        IsConnected = true;
                        uiDispatcher.BeginInvoke(new Action(() => { Connected?.Invoke(); }));
                        while (IsConnected) ReceiveMessage();
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    Disconnect();
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
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
                        for (Int32 attempts = 1; attempts <= 10 && serverIP == null; attempts++)
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
            catch (Exception)
            {
                return null;
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
            catch (Exception)
            {
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
                LostConnection?.Invoke();
            }));
        }
        /// <summary>
        /// Метод приём операции.
        /// </summary>
        private void ReceiveMessage()
        {
            if (socket.Poll(10000, SelectMode.SelectError))
                throw new Exception();
            else if (socket.Poll(10000, SelectMode.SelectRead))
            {
                Byte[] buffer = new Byte[256];
                StringBuilder message = new StringBuilder();

                do
                {
                    Int32 byteCount = socket.Receive(buffer);
                    message.Append(Encoding.UTF8.GetString(buffer), 0, byteCount);
                }
                while (socket.Available > 0);

                if (!String.IsNullOrWhiteSpace(message.ToString()))
                {
                    Operation operation = JsonSerializer.Deserialize<Operation>(message.ToString());
                    IncomingOperations.Enqueue(operation);
                }
                else throw new Exception();
            }
        }

        private void OnPropertyChanged([CallerMemberName] String prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
