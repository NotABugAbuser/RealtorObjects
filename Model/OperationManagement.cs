using RealtorObjects.View;
using RealtyModel.Event;
using RealtyModel.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        public LoggedEventHandler Logged;
        private Dispatcher dispatcher;
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
            try
            {
                if (operation.OperationParameters.Direction == OperationDirection.Identity)
                {
                    if (operation.IsSuccessfully)
                        HandleSuccessfulIdentity(operation);
                    else
                        HandleUnsuccessfulIdentity(operation);
                }
                else if (operation.OperationParameters.Direction == OperationDirection.Realty)
                {
                    if (operation.IsSuccessfully)
                        HandleSuccessfulRealty(operation);
                    else
                        HandleSuccessfulRealty(operation);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void HandleSuccessfulIdentity(Operation operation)
        {
            switch (operation.OperationParameters.Type)
            {
                case OperationType.Login:
                    {
                        if (operation.Name == credential.Name)
                            credential.OnLoggedIn();
                        break;
                    }
                case OperationType.Logout:
                    {
                        break;
                    }
                case OperationType.Register:
                    {
                        break;
                    }
                case OperationType.ToFire:
                    {
                        break;
                    }
            }
        }
        private void HandleUnsuccessfulIdentity(Operation operation)
        {
            switch (operation.OperationParameters.Type)
            {
                case OperationType.Login:
                    {
                        //LoginVM.Message = "Логин не был успешным";
                        break;
                    }
                case OperationType.Logout:
                    {
                        //LoginVM.Message = "Логаут не был успешным";
                        break;
                    }
                case OperationType.Register:
                    {
                        //LoginVM.Message = "Регистрация не была успешной";
                        break;
                    }
                case OperationType.ToFire:
                    {
                        //LoginVM.Message = "Удаление учётки не было успешным";
                        break;
                    }
            }
        }
        private void HandleSuccessfulRealty(Operation operation)
        {
            switch (operation.OperationParameters.Type)
            {
                case OperationType.Add:
                    {

                        break;
                    }
                case OperationType.Change:
                    {

                        break;
                    }
                case OperationType.Remove:
                    {

                        break;
                    }
                case OperationType.Update:
                    {

                        break;
                    }
            }
        }
        private void HandleUnsuccessfullReality(Operation operation)
        {
            switch (operation.OperationParameters.Type)
            {
                case OperationType.Add:
                    {
                        //Сообщить о неуспешности
                        break;
                    }
                case OperationType.Change:
                    {
                        //Сообщить о неуспешности
                        break;
                    }
                case OperationType.Remove:
                    {
                        //Сообщить о неуспешности
                        break;
                    }
                case OperationType.Update:
                    {
                        //Сообщить о неуспешности
                        break;
                    }
            }
        }
    }
}
