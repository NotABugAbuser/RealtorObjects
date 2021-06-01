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
using Action = RealtyModel.Model.Operations.Action;
using RealtyModel.Service;
using System.Windows;

namespace RealtorObjects.Model
{
    public class Client
    {
        private static IPAddress serverIp = IPAddress.Parse("127.0.0.1");
        private Dispatcher uiDispatcher = null;
        public Client(Dispatcher dispatcher) {
            uiDispatcher = dispatcher;
        }
        public static bool Login(Credential credential) {
            TcpClient client = new TcpClient();
            client.Connect(serverIp, 15000);
            NetworkStream network = client.GetStream();
            Operation operation = new Operation(Action.Login);
            operation.Data = BinarySerializer.Serialize(credential);
            NetworkTransfer.SendOperation(operation, network);
            
            Response response = NetworkTransfer.ReceiveResponse(network);
            bool isSuccessful = BinarySerializer.Deserialize<bool>(response.Data);
            OperationNotification.Notify(response.Code);
            return isSuccessful;
        }
        public static LocationOptions RequestLocationOptions() {
            TcpClient client = new TcpClient();
            client.Connect(serverIp, 15000);
            NetworkStream network = client.GetStream();
            Operation operation = new Operation(Action.Request, Target.Locations);
            NetworkTransfer.SendOperation(operation, network);

            Response response = NetworkTransfer.ReceiveResponse(network);
            LocationOptions locationOptions = BinarySerializer.Deserialize<LocationOptions>(response.Data);
            OperationNotification.Notify(response.Code);
            return locationOptions;
        }
    }
}
