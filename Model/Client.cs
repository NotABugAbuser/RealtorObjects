using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;
using RealtyModel.Service;
using Action = RealtyModel.Model.Operations.Action;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.Model
{
    public class Client
    {
        private static string name = "Unknown user";
        private static IPAddress serverIp;

        public static string Name {
            get => name; 
            set => name = value;
        }

        private static NetworkStream Connect() {

            TcpClient client = new TcpClient();
            if (Debugger.IsAttached) {
                serverIp = IPAddress.Parse("192.168.8.102");
            } else {
                serverIp = IPAddress.Parse("192.168.1.250");
            }
            client.Connect(serverIp, 15000);
            NetworkStream network = client.GetStream();
            return network;
        }
        public static ErrorCode Register(Credential credentials) {
            NetworkStream network = Connect();
            Operation operation = new Operation(Action.Register, BinarySerializer.Serialize(credentials), Name);
            Transfer.SendOperation(operation, network);

            return Transfer.ReceiveResponse(network).Code;
        }
        public static Album RequestAlbum(int id) {
            NetworkStream network = Connect();
            Operation operation = new Operation(Action.Request, Target.Album, BinarySerializer.Serialize(id), Name);
            Transfer.SendOperation(operation, network);

            Response response = Transfer.ReceiveResponse(network);
            OperationNotification.Notify(response.Code);
            return BinarySerializer.Deserialize<Album>(response.Data);
        }
        public static async Task<ObservableCollection<Byte[]>> RequestAlbumAsync(int id) {
            return await Task<ObservableCollection<Byte[]>>.Run(() => {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Request, Target.Album, BinarySerializer.Serialize(id), Name);
                Transfer.SendOperation(operation, network);

                Response response = Transfer.ReceiveResponse(network);
                OperationNotification.Notify(response.Code);
                return BinarySerializer.Deserialize<ObservableCollection<byte[]>>(response.Data);
            });
        }
        public static void AddFlat(Flat flat) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Add, Target.Flat, BinarySerializer.Serialize(flat), Name);
                Transfer.SendOperation(operation, network);
                OperationNotification.SuccessfulNotify(Transfer.ReceiveResponse(network).Code, $"{flat.GeneralInfo.ObjectType} успешно внесена в базу данных");
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
            }
        }
        public static void AddHouse(House house) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Add, Target.House, BinarySerializer.Serialize(house), Name);
                Transfer.SendOperation(operation, network);
                OperationNotification.SuccessfulNotify(Transfer.ReceiveResponse(network).Code, $"{house.GeneralInfo.ObjectType} успешно внесен в базу данных");
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
            }
        }
        public static void UpdateFlat(Flat flat) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Update, Target.Flat, BinarySerializer.Serialize(flat), Name);
                Transfer.SendOperation(operation, network);
                OperationNotification.Notify(Transfer.ReceiveResponse(network).Code);
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
            }
        }
        public static void UpdateHouse(House house) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Update, Target.House, BinarySerializer.Serialize(house), Name);
                Transfer.SendOperation(operation, network);
                OperationNotification.Notify(Transfer.ReceiveResponse(network).Code);
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
            }
        }
        public static bool Login(Credential credential) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Login, BinarySerializer.Serialize(credential), Name);
                Transfer.SendOperation(operation, network);

                Response response = Transfer.ReceiveResponse(network);
                bool isSuccessful = BinarySerializer.Deserialize<bool>(response.Data);
                Application.Current.Dispatcher.Invoke(() => {
                    OperationNotification.Notify(response.Code);
                });
                return isSuccessful;
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
                return false;
            }
        }
        public static List<BaseRealtorObject> RequestRealtorObjects(Filter filter) {
            NetworkStream network = Connect();
            Operation operation = new Operation(Action.Request, Target.RealtorObjects, BinarySerializer.Serialize(filter), Name);
            Transfer.SendOperation(operation, network);

            Response response = Transfer.ReceiveResponse(network);
            Tuple<Flat[], House[]> objects = BinarySerializer.Deserialize<Tuple<Flat[], House[]>>(response.Data);
            List<BaseRealtorObject> bros = new List<BaseRealtorObject>();
            bros.AddRange(objects.Item1);
            bros.AddRange(objects.Item2);
            Debug.WriteLine(bros.Count);
            OperationNotification.Notify(response.Code);
            return bros;
        }
        public static Street[] RequestStreets() {
            NetworkStream network = Connect();
            Operation operation = new Operation(Action.Request, Target.Locations,Name);
            Transfer.SendOperation(operation, network);

            Response response = Transfer.ReceiveResponse(network);
            Street[] streets = BinarySerializer.Deserialize<Street[]>(response.Data);
            OperationNotification.Notify(response.Code);
            return streets;
        }
    }
}
