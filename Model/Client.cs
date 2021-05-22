using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using RealtyModel.Model;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using RealtyModel.Events.Network;
using RealtyModel.Model.Operations;

namespace RealtorObjects.Model
{
    public class Client : INotifyPropertyChanged
    {
        #region Fileds
        private object sendLocker = new object();
        private Boolean isReceiving = false;
        private IPAddress serverIp = null;
        private TcpClient client = new TcpClient();
        private NetworkStream stream = null;
        private Dispatcher uiDispatcher = null;
        public event ConnectingEventHandler Connecting;
        public event ConnectedEventHandler Connected;
        public event DisconnectedEventHandler Disconnected;
        public event PropertyChangedEventHandler PropertyChanged;

        public Boolean IsReceiving
        {
            get => isReceiving;
            private set
            {
                isReceiving = value;
                OnPropertyChanged();
            }
        }
        public Boolean IsConnected { get; set; }
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
                Connecting?.Invoke(this, new ConnectingEventArgs());
                Debug.WriteLine($"{DateTime.Now} HAS STARTED TO CONNECT");
                IsTryingToConnect = true;
                FindServerIP();
                try
                {
                    if (serverIp != null)
                    {
                        client.Connect(serverIp, 15000);
                        stream = client.GetStream();
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
            IsReceiving = true;
            Debug.WriteLine($"{DateTime.Now} HAS STARTED TO RECEIVE BYTES");
            await Task.Run(() =>
            {
                try
                {
                    while (IsReceiving)
                    {
                        if (stream.DataAvailable)
                        {
                            List<byte> byteList = new List<byte>();
                            int size = GetSize();
                            bool isSuccessful = ReceiveData(byteList, size);
                            HandleResponseAsync(byteList.ToArray(), size);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"{DateTime.Now} (ReceiveAsync) {ex.Message}");
                }
                finally
                {
                    Debug.WriteLine($"{DateTime.Now} HAS FINISHED TO RECEIVE BYTES");
                }
            });
        }
        private bool ReceiveData(List<byte> byteList, int size)
        {
            try
            {
                while (byteList.Count < size)
                {
                    byte[] buffer = new byte[8192];
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    Debug.WriteLine($"Receive {bytes} bytes");
                    byte[] receivedData = new byte[bytes];
                    Array.Copy(buffer, receivedData, bytes);
                    byteList.AddRange(receivedData);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"(ReceiveData) {ex.Message}");
                return false;
            }
        }
        private int GetSize()
        {
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, buffer.Length);
            return BitConverter.ToInt32(buffer, 0);
        }
        private async void HandleResponseAsync(Byte[] data, int expectedSize)
        {
            await Task.Run(() =>
            {
                try
                {
                    Operation operation = BinarySerializer.Deserialize<Operation>(data);
                    if (data.Length == expectedSize)
                        Debug.WriteLine($"{DateTime.Now}RECEIVED {expectedSize} BYTES {operation.Number} - {operation.Parameters.Direction} {operation.Parameters.Action} {operation.Parameters.Target}");
                    else
                        Debug.WriteLine($"{DateTime.Now}RECEIVED WRONG BYTE COUNT: data - {data.Length} OF {expectedSize}");
                    Debug.WriteLine($"{DateTime.Now} {operation.Number} - {operation.Parameters.Direction} {operation.Parameters.Action} {operation.Parameters.Target}");
                    IncomingOperations.Enqueue(operation);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"\n{DateTime.Now} ERROR (HandleResponseAsync) {ex.Message}\n");
                }
            });
        }
        private async void SendAsync()
        {
            await Task.Run(() =>
            {
                lock (sendLocker)
                {
                    if (OutcomingOperations != null && OutcomingOperations.Count > 0)
                    {
                        try
                        {
                            Operation operation = OutcomingOperations.Dequeue();
                            operation.Number = (Guid.NewGuid()).ToString();
                            Byte[] data = BinarySerializer.Serialize(operation);
                            Byte[] dataSize = BitConverter.GetBytes(data.Length);

                            stream.Write(dataSize, 0, 4);
                            stream.Write(data, 0, data.Length);
                            Debug.WriteLine($"{DateTime.Now} has SENT {data.Length} bytes {operation.Number} - {operation.Parameters.Direction} {operation.Parameters.Action} {operation.Parameters.Target}");
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
                    IsReceiving = false;
                    Task.Delay(1000).Wait();
                    stream.Close();
                    stream.Dispose();
                    client.Close();
                    IsConnected = false;
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
