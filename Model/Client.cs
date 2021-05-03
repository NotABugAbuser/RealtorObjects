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
    public class Client : INotifyPropertyChanged
    {
        #region Fileds
        private object socketSendLocker = new object();
        private object streamSendLocker = new object();
        private Boolean isConnected = false;
        private Boolean isTryingToConnect = false;
        private IPAddress serverIp = null;
        private Socket socket = null;
        private NetworkStream stream = null;
        private Dispatcher uiDispatcher = null;
        public delegate void ConnectedEventHandler();
        public delegate void LostConnectionEventHandler();
        public event ConnectedEventHandler Connected;
        public event LostConnectionEventHandler LostConnection;
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
        public Boolean IsTryingToConnect
        {
            get => isTryingToConnect;
            private set
            {
                isTryingToConnect = value;
                OnPropertyChanged();
            }
        }
        public OperationQueue IncomingOperations
        {
            get; private set;
        }
        public OperationQueue OutcomingOperations
        {
            get; private set;
        }
        #endregion

        public Client()
        {

        }
        public Client(Dispatcher dispatcher)
        {
            uiDispatcher = dispatcher;
            IncomingOperations = new OperationQueue();
            OutcomingOperations = new OperationQueue();
            //OutcomingOperations.Enqueued += (s, e) => SendOverSocket();
            OutcomingOperations.Enqueued += (s, e) => SendOverStream();
        }

        public async void ConnectAsync()
        {
            await Task.Run(() =>
            {
                Debug.WriteLine($"{DateTime.Now} has started to connect");
                IsTryingToConnect = true;
                FindServerIPAddress();
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    if (serverIp != null)
                    {
                        IPEndPoint iPEndPoint = new IPEndPoint(serverIp, 8005);
                        socket.Connect(iPEndPoint);
                        stream = new NetworkStream(socket, true);
                        Debug.WriteLine($"{DateTime.Now} has CONNECTED to {serverIp}");
                        IsTryingToConnect = false;
                        IsConnected = true;
                        uiDispatcher.BeginInvoke(new Action(() => { Connected?.Invoke(); }));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now} (ConnectAsync){ex.Message}");
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
                            DisconnectAsync();
                            break;
                        }
                    }
                }
                catch (ObjectDisposedException)
                {
                    DisconnectAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now} (ConnectAsync){ex.Message}");
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
        private void FindServerIPAddress()
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
                Debug.WriteLine($"{DateTime.Now} (FindServerIPAddress){ex.Message}");
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

        public async void ReceiveOverStreamAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    Debug.WriteLine($"{DateTime.Now} has started to receive operations");
                    while (IsConnected)
                    {
                        if (stream.DataAvailable)
                        {
                            Byte[] buffer = new Byte[8192];
                            StringBuilder message = new StringBuilder();
                            Int32 bytes = 0;
                            String chunk;
                            do
                            {
                                Int32 byteCount = stream.Read(buffer, 0, buffer.Length);
                                bytes += byteCount;
                                chunk = Encoding.UTF8.GetString(buffer);
                                message.Append(chunk, 0, byteCount);
                                Debug.WriteLine($"{DateTime.Now} received {bytes}bytes");
                            }
                            while (stream.DataAvailable);
                            Operation operation = JsonSerializer.Deserialize<Operation>(message.ToString());
                            Debug.WriteLine($"{DateTime.Now} received {bytes}bytes {operation.OperationParameters.Direction} {operation.OperationParameters.Type} {operation.OperationParameters.Target} {operation.IsSuccessfully}");
                            IncomingOperations.Enqueue(operation);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now} {ex.Message}");
                }
            });
        }
        private void SendOverStream()
        {
            lock (streamSendLocker)
            {
                try
                {
                    Operation operation = OutcomingOperations.Dequeue();
                    operation.OperationNumber = Guid.NewGuid();
                    String json = JsonSerializer.Serialize<Operation>(operation);
                    Byte[] data = Encoding.UTF8.GetBytes(json);
                    stream.Write(data, 0, data.Length);
                    Debug.WriteLine($"{DateTime.Now} sent {data.Length}kbytes {operation.OperationParameters.Direction} {operation.OperationParameters.Type} {operation.OperationParameters.Target}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now} {ex.Message}");
                }
            }
        }
        public void SendDisconnect()
        {
            try
            {
                String json = JsonSerializer.Serialize<Operation>(new Operation() { Data = "0x00", OperationNumber = Guid.NewGuid() });
                Byte[] data = Encoding.UTF8.GetBytes(json);
                stream.Write(data, 0, data.Length);
                Debug.WriteLine($"{DateTime.Now} sent {data.Length}kbytes DISCONNECT");
                DisconnectAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{DateTime.Now} {ex.Message}");
            }
        }
        public async void DisconnectAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    serverIp = null;
                    IsConnected = false;
                    Task.Delay(1000).Wait();
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    socket.Dispose();
                    stream.Close();
                    stream.Dispose();
                    Debug.WriteLine($"{DateTime.Now} has DISCONNECTED");
                    uiDispatcher.BeginInvoke(new Action(() => { LostConnection?.Invoke(); }));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now} {ex.Message}");
                }
            });
        }
        private void OnPropertyChanged([CallerMemberName] String prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }



        public Task ReceiveOverSocketAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    while (IsConnected)
                    {
                        if (socket.Poll(100000, SelectMode.SelectRead))
                        {
                            StringBuilder message = new StringBuilder();
                            do
                            {
                                Byte[] buffer = new Byte[1024];
                                Int32 byteCount = socket.Receive(buffer);
                                message.Append(Encoding.UTF8.GetString(buffer), 0, byteCount);
                            }
                            while (socket.Available > 0);

                            Operation operation = JsonSerializer.Deserialize<Operation>(message.ToString());
                            IncomingOperations.Enqueue(operation);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now} {ex.Message}");
                }
            });
        }
        private void SendOverSocket()
        {
            lock (socketSendLocker)
            {
                try
                {
                    Operation operation = OutcomingOperations.Dequeue();
                    String json = JsonSerializer.Serialize<Operation>(operation);
                    Byte[] data = Encoding.UTF8.GetBytes(json);
                    socket.Send(data);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
