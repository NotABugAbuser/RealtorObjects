using RealtorObjects.View;
using RealtyModel.Event;
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
        private Dispatcher dispatcher;
        public event UpdateFlatEventHandler UpdateFlat;
        public event DeleteFlatEventHandler DeleteFlat;
        public event UpdateHouseEventHandler UpdateHouse;
        public event DeleteHouseEventHandler DeleteHouse;

        public OperationManagement()
        {

        }
        public OperationManagement(Client client, Credential credential, Dispatcher dispatcher)
        {
            this.client = client;
            this.credential = credential;
            this.dispatcher = dispatcher;
        }

        public async void AwaitOperationAsync()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    while (client.IncomingOperations.Count > 0)
                        HandleResponse(client.IncomingOperations.Dequeue());
                    Thread.Sleep(100);
                }
            });
        }

        public void Login(String name, String password)
        {
            credential.Name = name;
            credential.Password = password;
            client.SendMessage(new Operation(name, password, OperationDirection.Identity, OperationType.Login));
        }
        public void Register(String name, String password)
        {
            credential.Name = name;
            credential.Password = password;
            client.SendMessage(new Operation(name, password, OperationDirection.Identity, OperationType.Register));
        }
        public void SendFlat(Flat flat, OperationType operationType)
        {
            String json = JsonSerializer.Serialize<Flat>(flat);
            client.SendMessage(new Operation(credential.Name, json, OperationDirection.Realty, operationType));
        }

        private void HandleResponse(Operation operation)
        {
            if (operation.OperationParameters.Direction == OperationDirection.Identity)
                HandleIdentityResponse(operation);
            else if (operation.OperationParameters.Direction == OperationDirection.Realty)
                HandleRealtyResponse(operation);
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
                if (operation.OperationParameters.Target == TargetType.Flat)
                {
                    if (operation.OperationParameters.Type == OperationType.Add || operation.OperationParameters.Type == OperationType.Change)
                        UpdateFlat?.Invoke(this, new UpdateFlatEventArgs(JsonSerializer.Deserialize<Flat>(operation.Data)));
                    else if (operation.OperationParameters.Type == OperationType.Remove)
                        DeleteFlat?.Invoke(this, new DeleteFlatEventArgs(JsonSerializer.Deserialize<Flat>(operation.Data)));
                }
                else if (operation.OperationParameters.Target == TargetType.House)
                {
                    if (operation.OperationParameters.Type == OperationType.Add || operation.OperationParameters.Type == OperationType.Change)
                        UpdateHouse?.Invoke(this, new UpdateHouseEventArgs(JsonSerializer.Deserialize<House>(operation.Data)));
                    else if (operation.OperationParameters.Type == OperationType.Remove)
                        DeleteHouse?.Invoke(this, new DeleteHouseEventArgs(JsonSerializer.Deserialize<House>(operation.Data)));
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
