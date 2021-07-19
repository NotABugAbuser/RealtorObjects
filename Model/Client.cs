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
using Action = RealtyModel.Model.Operations.Action;
using System.Threading.Tasks;
using System.Windows;
using System.Net.NetworkInformation;
using RealtyModel.Model.Tools;
using System.Runtime.Serialization;

namespace RealtorObjects.Model
{
    public class Client
    {
        private static string name = "Unknown user";
        private static IPAddress serverIp = IPAddress.Parse("192.168.8.100");

        public static string Name {
            get => name;
            set => name = value;
        }
        public static IPAddress ServerIp {
            get => serverIp; set => serverIp = value;
        }
        public static bool CanConnect() {
            bool canConnect = false;
            Ping pinger = null;
            try {
                pinger = new Ping();
                PingReply reply = pinger.Send(ServerIp, 200);
                canConnect = reply.Status == IPStatus.Success;
            } catch (PingException) {
                canConnect = false;
            } finally {
                if (pinger != null) {
                    pinger.Dispose();
                }
            }
            if (!canConnect) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
            }
            return canConnect;
        }
        private static NetworkStream Connect() {
            TcpClient client = new TcpClient();
            client.Connect(ServerIp, 15000);
            NetworkStream network = client.GetStream();
            return network;
        }
        public static List<BaseRealtorObject> RequestCallableObjects() {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Request, Target.Calleable, name);
                Transfer.SendOperation(operation, network);


                Response response = Transfer.ReceiveResponse(network);
                Tuple<Flat[], House[]> objects = BinarySerializer.Deserialize<Tuple<Flat[], House[]>>(response.Data);
                List<BaseRealtorObject> bros = new List<BaseRealtorObject>();
                bros.AddRange(objects.Item1);
                bros.AddRange(objects.Item2);
                OperationNotification.Notify(response.Code);
                return bros;
            } catch (SerializationException) {
                OperationNotification.Notify(ErrorCode.Serialization);
                return new List<BaseRealtorObject>();
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
                return new List<BaseRealtorObject>();
            } catch (Exception) {
                OperationNotification.Notify(ErrorCode.Unknown);
                return new List<BaseRealtorObject>();
            }
        }
        public static List<Agent> RequestAgents() {
            NetworkStream network = Connect();
            Operation operation = new Operation(Action.Request, Target.Agent, Name);
            Transfer.SendOperation(operation, network);

            Response response = Transfer.ReceiveResponse(network);
            SymmetricEncryption encrypted = BinarySerializer.Deserialize<SymmetricEncryption>(response.Data);
            OperationNotification.Notify(response.Code);
            return encrypted.Decrypt<List<Agent>>();
        }
        public static bool UpdateAgents(List<Agent> agents) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Update, Target.Agent, new SymmetricEncryption(agents).Encrypt<List<Agent>>(), Name);
                Transfer.SendOperation(operation, network);

                Response response = Transfer.ReceiveResponse(network);
                OperationNotification.Notify(response.Code);
                return BinarySerializer.Deserialize<bool>(response.Data);
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
                return false;
            }
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

                Response response = Transfer.ReceiveResponse(network);
                OperationNotification.Notify(response.Code);
            } catch (SerializationException) {
                OperationNotification.Notify(ErrorCode.Serialization);
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
            } catch (Exception) {
                OperationNotification.Notify(ErrorCode.Unknown);
            }
        }
        public static void AddHouse(House house) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Add, Target.House, BinarySerializer.Serialize(house), Name);
                Transfer.SendOperation(operation, network);

                Response response = Transfer.ReceiveResponse(network);
                OperationNotification.Notify(response.Code);
            } catch (SerializationException) {
                OperationNotification.Notify(ErrorCode.Serialization);
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
            } catch (Exception) {
                OperationNotification.Notify(ErrorCode.Unknown);
            }
        }
        public static void UpdateFlat(Flat flat) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Update, Target.Flat, BinarySerializer.Serialize(flat), Name);
                Transfer.SendOperation(operation, network);

                Response response = Transfer.ReceiveResponse(network);
                OperationNotification.Notify(response.Code);
            } catch (SerializationException) {
                OperationNotification.Notify(ErrorCode.Serialization);
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
            } catch (Exception) {
                OperationNotification.Notify(ErrorCode.Unknown);
            }
        }
        public static void UpdateHouse(House house) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Update, Target.House, BinarySerializer.Serialize(house), Name);
                Transfer.SendOperation(operation, network);

                Response response = Transfer.ReceiveResponse(network);
                OperationNotification.Notify(response.Code);
            } catch (SerializationException) {
                OperationNotification.Notify(ErrorCode.Serialization);
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
            } catch (Exception) {
                OperationNotification.Notify(ErrorCode.Unknown);
            }
        }
        public static Tuple<bool, int> Login(Credential credential) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Login, new SymmetricEncryption(credential).Encrypt<Credential>(), Name);
                Transfer.SendOperation(operation, network);
                Response response = Transfer.ReceiveResponse(network);
                Tuple<bool, int> pair = BinarySerializer.Deserialize<Tuple<bool, int>>(response.Data);
                OperationNotification.Notify(response.Code);
                return pair;
            } catch (SerializationException) {
                OperationNotification.Notify(ErrorCode.Serialization);
                return new Tuple<bool, int>(false, 0);
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
                return new Tuple<bool, int>(false, 0);
            } catch (Exception) {
                OperationNotification.Notify(ErrorCode.Unknown);
                return new Tuple<bool, int>(false, 0);
            }
        }
        public static List<BaseRealtorObject> RequestRealtorObjects(Filtration filtration) {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Request, Target.RealtorObjects, BinarySerializer.Serialize(filtration), Name);
                Transfer.SendOperation(operation, network);
                Response response = Transfer.ReceiveResponse(network);
                Tuple<Flat[], House[]> objects = BinarySerializer.Deserialize<Tuple<Flat[], House[]>>(response.Data);
                List<BaseRealtorObject> bros = new List<BaseRealtorObject>();
                bros.AddRange(objects.Item1);
                bros.AddRange(objects.Item2);
                OperationNotification.Notify(response.Code);
                return bros;
            } catch (SerializationException) {
                OperationNotification.Notify(ErrorCode.Serialization);
                return new List<BaseRealtorObject>();
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
                return new List<BaseRealtorObject>();
            } catch (Exception) {
                OperationNotification.Notify(ErrorCode.Unknown);
                return new List<BaseRealtorObject>();
            }
        }
        public static string[] RequestStreets() {
            try {
                NetworkStream network = Connect();
                Operation operation = new Operation(Action.Request, Target.Locations, Name);
                Transfer.SendOperation(operation, network);

                Response response = Transfer.ReceiveResponse(network);
                string[] streets = BinarySerializer.Deserialize<string[]>(response.Data);
                OperationNotification.Notify(response.Code);
                return streets;
            } catch (SerializationException) {
                OperationNotification.Notify(ErrorCode.Serialization);
                return new string[0];
            } catch (SocketException) {
                OperationNotification.Notify(ErrorCode.ServerUnavailable);
                return new string[0];
            } catch (Exception) {
                OperationNotification.Notify(ErrorCode.Unknown);
                return new string[0];
            }
        }
    }
}
