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

namespace RealtorObjects.Model
{
    public class OperationManager
    {
        private Client client = new Client();

        public OperationManager()
        {

        }
        public OperationManager(Client client)
        {
            this.client = client;
        }



        public async void AwaitOperationAsync()
        {
            await Task.Run(() => {
                while (true)
                {
                    while (client.IncomingOperations.Count > 0)
                        HandleOperation(client.IncomingOperations.Dequeue());
                    Thread.Sleep(100);
                }
            });
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
                        //LoginVM.IsLoggedIn = true;
                        break;
                    }
                case OperationType.Logout:
                    {
                        //LoginVM.IsLoggedIn = false;
                        break;
                    }
                case OperationType.Register:
                    {
                        //LoginVM.Message = "Регистрация была успешной";
                        break;
                    }
                case OperationType.ToFire:
                    {
                        //LoginVM.Message = "Удаление учётки было успешным";
                        //Выбросить в окно регистрации
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
                        //HomeVM.LocationOptions - добавить новые объекты в списки
                        //LocationVM.LocationOptions - добавить новые объекты в списки
                        break;
                    }
                case OperationType.Change:
                    {
                        //HomeVM.LocationOptions - обновить по Id необходимые объекты в списках
                        //LocationVM.LocationOptions - обновить по Id необходимые объекты в списках
                        break;
                    }
                case OperationType.Remove:
                    {
                        //HomeVM.LocationOptions - удалить по Id необходимые объекты из списков
                        //LocationVM.LocationOptions - удалить по Id необходимые объекты из списков
                        break;
                    }
                case OperationType.Update:
                    {
                        //HomeVM.LocationOptions - получение всех списков от сервера
                        //LocationVM.LocationOptions - получение всех списков от сервера
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
