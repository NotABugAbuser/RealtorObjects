using RealtorObjects.Model;
using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Event;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            Task connection = Client.ConnectAsync();
            Task receive = connection.ContinueWith((a) => Client.ReceiveOverStreamAsync());
            receive.Wait();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            Client.Disconnect();
            base.OnExit(e);
        }
        private void InitializeMembers()
        {
            operationManagement = new OperationManagement(client, credential, Dispatcher);
            windowManagement = new WindowManagement(client, credential);
            windowManagement.Run();
        }
        private void BindEvents()
        {
            windowManagement.LoginFormVM.LoggingIn += (s, e) => operationManagement.Login(e.UserName, e.Password);
            windowManagement.LoginFormVM.Registering += (s, e) => operationManagement.Register(e.UserName, e.Password, e.Email);

            windowManagement.FlatFormVM.FlatCreated = (s, e) => operationManagement.SendFlat(e.Flat, OperationType.Add);
            windowManagement.FlatFormVM.FlatModified = (s, e) => operationManagement.SendFlat(e.Flat, OperationType.Change);
            //windowManagement.HouseFormVM.HouseCreated = (s, e) => operationManagement.SendFlat(e.House, OperationType.Add);

            operationManagement.ReceivedDbUpdate += (s, e) => windowManagement.HomeVM.ReceiveDbUpdate(e);
            operationManagement.ReceivedFlat += (s, e) => windowManagement.HomeVM.AddFlat(e.Flat);
            operationManagement.ReceivedFlatUpdate += (s, e) => windowManagement.HomeVM.UpdateFlat(e.Flat);
            operationManagement.ReceivedFlatDeletion += (s, e) => windowManagement.HomeVM.DeleteFlat(e.Flat);
            operationManagement.ReceivedHouse += (s, e) => windowManagement.HomeVM.AddHouse(e.House);
            operationManagement.ReceivedHouseUpdate += (s, e) => windowManagement.HomeVM.UpdateHouse(e.House);
            operationManagement.ReceivedHouseDeletion += (s, e) => windowManagement.HomeVM.DeleteHouse(e.House);
        }
    }
}
