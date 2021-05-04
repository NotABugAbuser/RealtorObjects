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
using System.IO;
using RealtyModel.Service;
using RealtorObjects.Model.Event;

namespace RealtorObjects.Model
{
    public class Client : INotifyPropertyChanged
    {
        #region Fileds
        private object streamSendLocker = new object();
        private Boolean isConnected = false;
        private IPAddress serverIp = null;
        private Socket socket = null;
        private NetworkStream stream = null;
        private Dispatcher uiDispatcher = null;
        public event ConnectedEventHandler Connected;
        public event DisconnectedEventHandler Disconnected;
        public event PropertyChangedEventHandler PropertyChanged;

        public Boolean IsConnected
        {
            get => isConnected;
            private set
            {
                isConnected = value;
                OnPropertyChanged();
            }
        }
        public Boolean IsFirstConnection { get; set; }
        public Boolean IsTryingToConnect { get; set; }
        public OperationQueue IncomingOperations
        {
            get; private set;
        }
        public OperationQueue OutcomingOperations
        {
            get; private set;
        }
        #endregion

        public Client(Dispatcher dispatcher)
        {
            uiDispatcher = dispatcher;
            IncomingOperations = new OperationQueue();
            OutcomingOperations = new OperationQueue();
            OutcomingOperations.Enqueued += (s, e) => SendAsync();
        }

        public async void ConnectAsync()
        {
            await Task.Run(() =>
            {
                Debug.WriteLine($"{DateTime.Now} HAS STARTED TO CONNECT");
                IsTryingToConnect = true;
                FindServerIP();
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    if (serverIp != null)
                    {
                        IPEndPoint iPEndPoint = new IPEndPoint(serverIp, 8005);
                        socket.Connect(iPEndPoint);
                        stream = new NetworkStream(socket, true);
                        Debug.WriteLine($"{DateTime.Now} HAS CONNECTED TO {serverIp}");
                        IsTryingToConnect = false;
                        IsConnected = true;
                        uiDispatcher.BeginInvoke(new Action(() => { Connected?.Invoke(this, new ConnectedEventArgs()); }));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"\n{DateTime.Now} ERROR (ConnectAsync) {ex.Message}\n");
                    DisconnectAsync();
                }
            });
        }
        public async void CheckConnectionAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        if (GetSocketStatus())
                            Task.Delay(1000).Wait();
                        else
                        {
                            Debug.WriteLine($"\n{DateTime.Now}SOCKET WAS UNAVAILABLE\n");
                            DisconnectAsync();
                            break;
                        }
                    }
                }
                catch (ObjectDisposedException ex)
                {
                    Debug.WriteLine($"\n{DateTime.Now} SOCKET ERROR (CheckConnectionAsync) {ex.Message}\n");
                    DisconnectAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"\n{DateTime.Now} ERROR (CheckConnectionAsync) {ex.Message}\n");
                }
            });
            bool GetSocketStatus()
            {
                bool part1 = socket.Poll(1000, SelectMode.SelectRead);
                bool part2 = (socket.Available == 0);
                if (part1 && part2)
                    return false;
                else
                    return true;
            }
        }
        private void FindServerIP()
        {
            try
            {
                Debug.WriteLine($"{DateTime.Now} has started to look for server ip");
                Socket socket = null;
                IPAddress[] iPAddresses = GetLocalIPv4Addresses();
                do
                {
                    foreach (IPAddress iP in iPAddresses)
                    {
                        using (socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                        {
                            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);
                            socket.EnableBroadcast = true;
                            socket.ReceiveTimeout = 1000;
                            socket.Bind(new IPEndPoint(iP, 8080));
                            for (Int32 attempts = 1; attempts <= 10 && serverIp == null; attempts++)
                            {
                                socket.SendTo(new byte[] { 0x10 }, new IPEndPoint(IPAddress.Broadcast, 8080));
                                Int32 byteCount = 0;
                                byte[] buffer = new byte[socket.ReceiveBufferSize];
                                EndPoint endPoint = new IPEndPoint(IPAddress.None, 0);
                                if (socket.Poll(1000000, SelectMode.SelectRead))
                                {
                                    byteCount = socket.ReceiveFrom(buffer, ref endPoint);
                                    if (byteCount == 1 && buffer[0] == 0x20)
                                        serverIp = (endPoint as IPEndPoint).Address;
                                }
                                Thread.Sleep(200);
                            }
                            if (serverIp != null) break;
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                        }
                    }
                }
                while (serverIp == null && IsTryingToConnect);
                Debug.WriteLine($"{DateTime.Now} has found server ip at {serverIp}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"\n{DateTime.Now} ERROR (FindServerIP) {ex.Message}\n");
            }
        }
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

        public async void ReceiveAsync()
        {
            Debug.WriteLine($"{DateTime.Now} HAS STARTED TO RECEIVE BYTES");
            await Task.Run(() =>
            {
                try
                {
                    while (IsConnected)
                    {
                        if (socket.Available > 0)
                        {
                            byte[] buffer = new byte[256];
                            StringBuilder response = new StringBuilder();

                            do
                            {
                                socket.Receive(buffer);
                                String received = Encoding.UTF8.GetString(buffer);
                                if (received.Contains("<EOF>"))
                                {
                                    String[] ar = received.Split(new String[] { "<EOF>" }, StringSplitOptions.None);
                                    response.Append(ar[0]);
                                }
                                else response.Append(received);
                            }
                            while (socket.Available > 0);
                            Debug.WriteLine($"{DateTime.Now} has received {response}");
                            HandleResponseAsync(response.ToString());
                        }
                        Task.Delay(10).Wait();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now} (ReceiveOverStreamAsync) bytes {ex.Message}");
                }
                finally
                {
                    Debug.WriteLine($"{DateTime.Now} HAS FINISHED TO RECEIVE BYTES");
                }
            });
        }
        private async void HandleResponseAsync(String data)
        {
            Debug.WriteLine($"{DateTime.Now} has started to handle response");
            await Task.Run(() =>
            {
                try
                {
                    Operation operation = JsonSerializer.Deserialize<Operation>(data);
                    Debug.WriteLine($"{DateTime.Now} has finished to handle response");
                    IncomingOperations.Enqueue(operation);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"\n{DateTime.Now} ERROR (GetOperationAsync) {ex.Message}\n");
                }
            });
        }
        private async void SendAsync()
        {
            await Task.Run(() =>
            {
                lock (streamSendLocker)
                {
                    if (OutcomingOperations != null && OutcomingOperations.Count > 0)
                    {
                        try
                        {
                            Operation operation = OutcomingOperations.Dequeue();
                            operation.OperationNumber = Guid.NewGuid();
                            Debug.WriteLine($"{DateTime.Now} has started to send {operation.OperationNumber} {operation.Parameters.Target}");
                            Byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(operation) + "<EOF>");
                            socket.Send(data);
                            Debug.WriteLine($"{DateTime.Now} has sent {data.Length} bytes");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"\n{DateTime.Now} ERROR (SendAsync) {ex.Message}\n");
                        }
                        Task.Delay(100).Wait();
                    }
                }
            });
        }

        public async void DisconnectAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine($"{DateTime.Now} has started to disconnect");
                    serverIp = null;
                    IsConnected = false;
                    Task.Delay(1000).Wait();
                    socket.Shutdown(SocketShutdown.Both);
                    stream.Close();
                    stream.Dispose();
                    Debug.WriteLine($"{DateTime.Now} HAS DISCONNECTED");
                    uiDispatcher.BeginInvoke(new Action(() => { Disconnected?.Invoke(this, new DisconnectedEventArgs()); }));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"\n{DateTime.Now} ERROR (DisconnectAsync) {ex.Message}\n");
                }
            });
        }
        private void OnPropertyChanged([CallerMemberName] String prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
