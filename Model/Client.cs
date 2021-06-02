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
using RealtyModel.Model.Derived;
using System.Collections.ObjectModel;
using RealtyModel.Model.Base;

namespace RealtorObjects.Model
{
    public class Client
    {
        private static readonly IPAddress serverIp = IPAddress.Parse("127.0.0.1");
        private static NetworkStream Connect() {
            TcpClient client = new TcpClient();
            client.Connect(serverIp, 15000);
            NetworkStream network = client.GetStream();
            return network;
        }
        public static void AddFlat(Flat flat) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Add, Target.Flat, BinarySerializer.Serialize(flat));
                Transfer.SendOperation(operation, network);
                OperationNotification.Notify(Transfer.ReceiveResponse(network).Code);
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
            }
        }
        public static bool Login(Credential credential) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Login, BinarySerializer.Serialize(credential));
                Transfer.SendOperation(operation, network);

                Response response = Transfer.ReceiveResponse(network);
                bool isSuccessful = BinarySerializer.Deserialize<bool>(response.Data);
                OperationNotification.Notify(response.Code);
                return isSuccessful;
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
                return false;
            }
        }
        public static List<BaseRealtorObject> RequestRealtorObjects(Filter filter) {
            NetworkStream network = Connect();
            Operation operation = new Operation(Action.Request, Target.RealtorObjects, BinarySerializer.Serialize(filter));
            Transfer.SendOperation(operation, network);

            Response response = Transfer.ReceiveResponse(network);
            Tuple<Flat[], House[]> objects = BinarySerializer.Deserialize<Tuple<Flat[], House[]>>(response.Data);
            List<BaseRealtorObject> bros = new List<BaseRealtorObject>(objects.Item1);
            Debug.WriteLine(bros.Count);
            OperationNotification.Notify(response.Code);
            return bros;
        }
        public static LocationOptions RequestLocationOptions() {
            NetworkStream network = Connect();
            Operation operation = new Operation(Action.Request, Target.Locations);
            Transfer.SendOperation(operation, network);

            Response response = Transfer.ReceiveResponse(network);
            LocationOptions locationOptions = BinarySerializer.Deserialize<LocationOptions>(response.Data);
            OperationNotification.Notify(response.Code);
            return locationOptions;
        }
    }
}
