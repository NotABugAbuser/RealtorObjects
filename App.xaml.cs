using RealtorObjects.Model;
using RealtorObjects.View;
using RealtyModel.Model;
using System;
using System.Windows;
using System.Windows.Threading;

namespace RealtorObjects
{
    public partial class App : Application
    {
        private WindowManagement windowManagement;
        private OperationManagement operationManagement;
        private Client client = new Client(Dispatcher.CurrentDispatcher);
        private Credential credential = new Credential(Dispatcher.CurrentDispatcher);

        public Client Client
        {
            get => client;
            private set => client = value;
        }
        public Credential Credential
        {
            get => credential;
            private set => credential = value;
        }
        public WindowManagement WindowManagement
        {
            get => windowManagement;
            set => windowManagement = value;
        }
        public OperationManagement OperationManagement
        {
            get => operationManagement;
            private set => operationManagement = value;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeMembers();
            BindEvents();
            Client.ConnectAsync();
        }
        private void InitializeMembers()
        {
            operationManagement = new OperationManagement(client, credential, Dispatcher);
            windowManagement = new WindowManagement(client, credential);
            windowManagement.Run();
        }
        private void BindEvents()
        {
            Client.Connected += (s, e) => Client.CheckConnectionAsync();
            Client.Connected += (s, e) => Client.ReceiveAsync();
            Client.Connected += (s, e) => CheckClientStatus();

            windowManagement.LoginFormVM.LoggingIn += (s, e) => operationManagement.SendIdentityData(e.UserName, e.Password, OperationType.Login);
            windowManagement.LoginFormVM.Registering += (s, e) => operationManagement.SendIdentityData(e.UserName, $"{e.Password};{e.Email}", OperationType.Register);

            windowManagement.FlatFormVM.FlatCreated = (s, e) => operationManagement.SendRealtyData(e.Flat, OperationType.Add, TargetType.Flat);
            windowManagement.FlatFormVM.FlatModified = (s, e) => operationManagement.SendRealtyData(e.Flat, OperationType.Change, TargetType.Flat);

            operationManagement.ReceivedDbUpdate += (s, e) => windowManagement.HomeVM.ReceiveDbUpdate(e);
            operationManagement.ReceivedFlat += (s, e) => windowManagement.HomeVM.AddFlat(e.Flat);
            operationManagement.ReceivedFlatUpdate += (s, e) => windowManagement.HomeVM.UpdateFlat(e.Flat);
            operationManagement.ReceivedFlatDeletion += (s, e) => windowManagement.HomeVM.DeleteFlat(e.Flat);
            operationManagement.ReceivedHouse += (s, e) => windowManagement.HomeVM.AddHouse(e.House);
            operationManagement.ReceivedHouseUpdate += (s, e) => windowManagement.HomeVM.UpdateHouse(e.House);
            operationManagement.ReceivedHouseDeletion += (s, e) => windowManagement.HomeVM.DeleteHouse(e.House);
        }
        private void CheckClientStatus()
        {
            if (Client.IsFirstConnection || !windowManagement.HomeVM.HasUpdate)
                operationManagement.SendRealtyData(null, OperationType.Update, TargetType.All);
            else if (Credential.IsLoggedIn && !String.IsNullOrWhiteSpace(Credential.Name) && !String.IsNullOrWhiteSpace(Credential.Password))
                operationManagement.SendIdentityData(Credential.Name, Credential.Password, OperationType.Login);
            else windowManagement.OpenLoginForm();
        }
    }
}
