using RealtorObjects.View;
using RealtyModel.Event;
using RealtyModel.Event.RealtyEvents;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RealtorObjects.Model
{
    public class OperationManagement
    {
        private Client client = null;
        private Credential credential = null;
        private Dispatcher dispatcher = null;
        private object handleLocker = new object();

        public event ReceivedDbUpdateEventHandler ReceivedDbUpdate;
        public event ReceivedFlatEventHandler ReceivedFlat;
        public event ReceivedFlatUpdateEventHandler ReceivedFlatUpdate;
        public event ReceivedFlatDeletionEventHandler ReceivedFlatDeletion;
        public event ReceivedHouseEventHandler ReceivedHouse;
        public event ReceivedHouseUpdateEventHandler ReceivedHouseUpdate;
        public event ReceivedHouseDeletionEventHandler ReceivedHouseDeletion;

        public OperationManagement(Client client, Credential credential, Dispatcher dispatcher)
        {
            this.client = client;
            this.credential = credential;
            this.dispatcher = dispatcher;
            client.IncomingOperations.Enqueued += (s, e) => HandleResponse();
        }

        public void SendIdentityData(String name, String password, OperationType type)
        {
            credential.Name = name;
            credential.Password = password;
            client.OutcomingOperations.Enqueue(new Operation()
            {
                Name = name,
                Data = JsonSerializer.SerializeToUtf8Bytes(password),
                OperationNumber = Guid.NewGuid(),
                Parameters = new OperationParameters()
                {
                    Direction = OperationDirection.Identity,
                    Type = type,
                    Target = TargetType.Agent
                }
            });
        }
        public void SendRealtyData(object data, OperationType type, TargetType target)
        {
            client.OutcomingOperations.Enqueue(new Operation()
            {
                Name = credential.Name,
                Data = JsonSerializer.SerializeToUtf8Bytes<object>(data),
                OperationNumber = Guid.NewGuid(),
                Parameters = new OperationParameters()
                {
                    Direction = OperationDirection.Realty,
                    Type = type,
                    Target = target
                }
            });
        }

        private void HandleResponse()
        {
            lock (handleLocker)
            {
                Operation operation = client.IncomingOperations.Dequeue();
                if (operation.Parameters.Direction == OperationDirection.Identity)
                    HandleIdentityResponse(operation);
                else if (operation.Parameters.Direction == OperationDirection.Realty)
                    HandleRealtyResponse(operation);
            }
        }
        private void HandleIdentityResponse(Operation operation)
        {
            if (operation.Name == credential.Name && operation.IsSuccessfully)
            {
                if (operation.Parameters.Type == OperationType.Login) credential.OnLoggedIn();
                else if (operation.Parameters.Type == OperationType.Logout) credential.OnLoggedOut();
                else if (operation.Parameters.Type == OperationType.Register) credential.OnRegistered();
            }
            else if (operation.Name != credential.Name)
            {
                MessageBox.Show("Неправильное имя пользователя");
            }
            else if (!operation.IsSuccessfully)
            {
                if (operation.Parameters.Type == OperationType.Login)
                    MessageBox.Show("Операция входа не была успешна");
                else if (operation.Parameters.Type == OperationType.Logout)
                    MessageBox.Show("Операция выхода не была успешна");
                else if (operation.Parameters.Type == OperationType.Register)
                    MessageBox.Show("Операция регистрации не была успешна");
                else if (operation.Parameters.Type == OperationType.ToFire)
                    MessageBox.Show("Операция увольнения не была успешна");
            }
        }
        private void HandleRealtyResponse(Operation operation)
        {
            try
            {
                if (operation.IsSuccessfully)
                {
                    if (operation.Parameters.Target == TargetType.All || operation.Parameters.Target == TargetType.Album || operation.Parameters.Target == TargetType.None)
                        ReceivedDbUpdate?.Invoke(this, new ReceivedDbUpdateEventArgs(operation.Parameters.Target, operation.Data));
                    else if (operation.Parameters.Target == TargetType.Flat)
                    {
                        Flat flat = JsonSerializer.Deserialize<Flat>(operation.Data);
                        if (operation.Parameters.Type == OperationType.Add)
                            ReceivedFlat?.Invoke(this, new ReceivedFlatEventArgs(flat));
                        else if (operation.Parameters.Type == OperationType.Change)
                            ReceivedFlatUpdate?.Invoke(this, new ReceivedFlatUpdateEventArgs(flat));
                        else if (operation.Parameters.Type == OperationType.Remove)
                            ReceivedFlatDeletion?.Invoke(this, new ReceivedFlatDeletionEventArgs(flat));
                    }
                    else if (operation.Parameters.Target == TargetType.House)
                    {
                        House house = JsonSerializer.Deserialize<House>(operation.Data);
                        if (operation.Parameters.Type == OperationType.Add)
                            ReceivedHouse?.Invoke(this, new ReceivedHouseEventArgs(house));
                        else if (operation.Parameters.Type == OperationType.Change)
                            ReceivedHouseUpdate?.Invoke(this, new ReceivedHouseUpdateEventArgs(house));
                        else if (operation.Parameters.Type == OperationType.Remove)
                            ReceivedHouseDeletion?.Invoke(this, new ReceivedHouseDeletionEventArgs(house));
                    }
                }
                else
                {
                    MessageBox.Show("Операция не была успешна");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{DateTime.Now} OperationManagement-HandleRealtyResponse {ex.Message}");
            }
        }
    }
}
