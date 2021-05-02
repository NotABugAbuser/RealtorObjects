using RealtorObjects.View;
using RealtyModel.Event;
using RealtyModel.Event.RealtyEvents;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
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

        public OperationManagement()
        {

        }
        public OperationManagement(Client client, Credential credential, Dispatcher dispatcher)
        {
            this.client = client;
            this.credential = credential;
            this.dispatcher = dispatcher;
            client.IncomingOperations.Enqueued += (s, e) => HandleResponse();
        }

        public void Login(String name, String password)
        {
            credential.Name = name;
            credential.Password = password;
            client.OutcomingOperations.Enqueue(new Operation(name, password, OperationDirection.Identity, OperationType.Login));
        }
        public void Register(String name, String password, String email)
        {
            credential.Name = name;
            credential.Password = password;
            credential.Email = email;
            String json = JsonSerializer.Serialize<Credential>(credential);
            client.OutcomingOperations.Enqueue(new Operation(name, json, OperationDirection.Identity, OperationType.Register));
        }
        public void SendFlat(Flat flat, OperationType operationType)
        {
            String json = JsonSerializer.Serialize<Flat>(flat);
            client.OutcomingOperations.Enqueue(new Operation(credential.Name, json, OperationDirection.Realty, operationType));
        }

        private void HandleResponse()
        {
            lock (handleLocker)
            {
                Operation operation = client.IncomingOperations.Dequeue();
                if (operation.OperationParameters.Direction == OperationDirection.Identity)
                    HandleIdentityResponse(operation);
                else if (operation.OperationParameters.Direction == OperationDirection.Realty)
                    HandleRealtyResponse(operation);
            }
        }
        private void HandleIdentityResponse(Operation operation)
        {
            if (operation.Name == credential.Name && operation.IsSuccessfully)
            {
                if (operation.OperationParameters.Type == OperationType.Login) credential.OnLoggedIn();
                else if (operation.OperationParameters.Type == OperationType.Logout) credential.OnLoggedOut();
                else if (operation.OperationParameters.Type == OperationType.Register) credential.OnRegistered();
            }
            else if (operation.Name != credential.Name)
            {
                MessageBox.Show("Неправильное имя пользователя");
            }
            else if (!operation.IsSuccessfully)
            {
                if (operation.OperationParameters.Type == OperationType.Login)
                    MessageBox.Show("Операция входа не была успешна");
                else if (operation.OperationParameters.Type == OperationType.Logout)
                    MessageBox.Show("Операция выхода не была успешна");
                else if (operation.OperationParameters.Type == OperationType.Register)
                    MessageBox.Show("Операция регистрации не была успешна");
                else if (operation.OperationParameters.Type == OperationType.ToFire)
                    MessageBox.Show("Операция увольнения не была успешна");
            }
        }
        private void HandleRealtyResponse(Operation operation)
        {
            try
            {
                if (operation.OperationParameters.Target == TargetType.All)
                    ReceivedDbUpdate?.Invoke(this, new ReceivedDbUpdateEventArgs(operation.OperationParameters.Target, operation.Data));
                else if (operation.OperationParameters.Target == TargetType.Flat)
                {
                    Flat flat = JsonSerializer.Deserialize<Flat>(operation.Data);
                    if (operation.OperationParameters.Type == OperationType.Add)
                        ReceivedFlat?.Invoke(this, new ReceivedFlatEventArgs(flat));
                    else if (operation.OperationParameters.Type == OperationType.Change)
                        ReceivedFlatUpdate?.Invoke(this, new ReceivedFlatUpdateEventArgs(flat));
                    else if (operation.OperationParameters.Type == OperationType.Remove)
                        ReceivedFlatDeletion?.Invoke(this, new ReceivedFlatDeletionEventArgs(flat));
                }
                else if (operation.OperationParameters.Target == TargetType.House)
                {
                    House house = JsonSerializer.Deserialize<House>(operation.Data);
                    if (operation.OperationParameters.Type == OperationType.Add)
                        ReceivedHouse?.Invoke(this, new ReceivedHouseEventArgs(house));
                    else if (operation.OperationParameters.Type == OperationType.Change)
                        ReceivedHouseUpdate?.Invoke(this, new ReceivedHouseUpdateEventArgs(house));
                    else if (operation.OperationParameters.Type == OperationType.Remove)
                        ReceivedHouseDeletion?.Invoke(this, new ReceivedHouseDeletionEventArgs(house));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{DateTime.Now} OperationManagement-HandleRealtyResponse {ex.Message}");
            }
        }
    }
}
