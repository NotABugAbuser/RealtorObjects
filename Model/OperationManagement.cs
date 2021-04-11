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
        public LoggedEventHandler Logged;
        public UpdateFlatEventHandler UpdateFlat;
        public DeleteFlatEventHandler DeleteFlat;

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
                        HandleOperation(client.IncomingOperations.Dequeue());
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

        private void HandleOperation(Operation operation)
        {
            if (operation.OperationParameters.Direction == OperationDirection.Identity && operation.IsSuccessfully)
            {
                if (operation.OperationParameters.Type == OperationType.Login) HandleLoginResponse(operation);
                //else if(operation.OperationParameters.Type == OperationType.Logout)
                //else if(operation.OperationParameters.Type == OperationType.Register)
                //else if(operation.OperationParameters.Type == OperationType.ToFire)
            }
            else if (operation.OperationParameters.Direction == OperationDirection.Realty && !operation.IsSuccessfully)
            {
                if (operation.OperationParameters.Type == OperationType.Add || operation.OperationParameters.Type == OperationType.Change) 
                    UpdateFlat?.Invoke(this, new UpdateFlatEventArgs(JsonSerializer.Deserialize<Flat>(operation.Data)));
                else if(operation.OperationParameters.Type == OperationType.Remove)
                    DeleteFlat?.Invoke(this, new DeleteFlatEventArgs(JsonSerializer.Deserialize<Flat>(operation.Data)));
            }
        }
        private void HandleLoginResponse(Operation operation)
        {
            if (operation.Name == credential.Name)
                credential.OnLoggedIn();
        }
    }
}
