using RealtyModel.Events.Identity;
using RealtyModel.Events.Realty;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;
using RealtyModel.Model.RealtyObjects;
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

namespace RealtorObjects.Model
{
    public class OperationManagement
    {
        private Client client = null;
        private Credential credential = null;
        private object handleLocker = new object();

        public event ReceivedFlatEventHandler ReceivedFlat;
        public event ReceivedPhotoEventHandler ReceivedPhoto;

        public OperationManagement(Client client, Credential credential)
        {
            this.client = client;
            this.credential = credential;
            client.IncomingOperations.Enqueued += (s, e) => HandleResponse();
        }

        public void SendRequest(object data, Parameters parameters)
        {
            Operation operation = new Operation()
            {
                Number = (Guid.NewGuid()).ToString(),
                Parameters = parameters
            };
            operation.Parameters.Initiator = Initiator.User;
            if (parameters.Direction == Direction.Identity)
                SendIdentityRequest(data, operation);
            else SendRealtyRequest(data, operation);
        }
        private void SendIdentityRequest(object data, Operation operation)
        {
            if (operation.Parameters.Action == Act.Login)
            {
                LoggingInEventArgs e = (LoggingInEventArgs)data;
                credential.Name = e.UserName;
                credential.Password = e.Password;
                operation.Name = e.UserName;
                operation.Data = e.Password;
            }
            else if (operation.Parameters.Action == Act.Register)
            {
                RegisteringEventArgs e = (RegisteringEventArgs)data;
                operation.Name = e.UserName;
                operation.Data = JsonSerializer.Serialize(new Credential() { Name = e.UserName, Password = e.Password, Email = e.Email });
            }
            client.OutcomingOperations.Enqueue(operation);
        }
        private void SendRealtyRequest(object data, Operation operation)
        {
            operation.Name = credential.Name;
            if (operation.Parameters.Target == Target.Photo)
                operation.Data = $"{((Photo)data).Guid}<GUID>{JsonSerializer.Serialize((Photo)data)}";
            else operation.Data = JsonSerializer.Serialize(data);
            client.OutcomingOperations.Enqueue(operation);
        }

        private void HandleResponse()
        {
            lock (handleLocker)
            {
                Operation operation = client.IncomingOperations.Dequeue();
                if (operation.Parameters.Direction == Direction.Identity)
                    HandleIdentityResponse(operation);
                else if (operation.Parameters.Direction == Direction.Realty)
                    HandleRealtyResponse(operation);
            }
        }
        private void HandleIdentityResponse(Operation operation)
        {
            if (operation.Name == credential.Name && operation.IsSuccessfully)
            {
                if (operation.Parameters.Action == Act.Login) credential.OnLoggedIn();
                else if (operation.Parameters.Action == Act.Logout) credential.OnLoggedOut();
                else if (operation.Parameters.Action == Act.Register) credential.OnRegistered();
            }
            else if (operation.Name != credential.Name)
            {
                MessageBox.Show("Неправильное имя пользователя");
            }
            else if (!operation.IsSuccessfully)
            {
                if (operation.Parameters.Action == Act.Login)
                    MessageBox.Show("Операция входа не была успешна");
                else if (operation.Parameters.Action == Act.Logout)
                    MessageBox.Show("Операция выхода не была успешна");
                else if (operation.Parameters.Action == Act.Register)
                    MessageBox.Show("Операция регистрации не была успешна");
                else if (operation.Parameters.Action == Act.Dismiss)
                    MessageBox.Show("Операция увольнения не была успешна");
            }
        }
        private void HandleRealtyResponse(Operation operation)
        {
            try
            {
                if (operation.IsSuccessfully)
                {
                    if (operation.Parameters.Target == Target.Flat)
                        ReceivedFlat?.Invoke(this, new ReceivedFlatEventArgs(JsonSerializer.Deserialize<Flat>(operation.Data), operation.Parameters.Action, operation.Parameters.Initiator));
                    else if (operation.Parameters.Target == Target.Photo)
                        ReceivedPhoto?.Invoke(this, new ReceivedPhotoEventArgs(operation.Parameters.Action, operation.Data));
                }
                else MessageBox.Show("Операция не была успешна");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{DateTime.Now} OperationManagement-HandleRealtyResponse {ex.Message}\n{JsonSerializer.Serialize(operation)}");
            }
        }
    }
}
