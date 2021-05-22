using RealtorObjects.Model;
using RealtyModel.Events.Identity;
using RealtyModel.Events.Realty;
using RealtyModel.Model;
using RealtyModel.Model.Operations;
using RealtyModel.Model.RealtyObjects;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace RealtorObjects
{
    public partial class App : Application
    {
        #region Fields and Properties
        private Client client = new Client(Dispatcher.CurrentDispatcher);
        private Credential credential = new Credential(Dispatcher.CurrentDispatcher);
        private WindowManagement windowManagement;
        private OperationManagement operationManagement;

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
        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeMembers();
            BindEvents();
            Client.ConnectAsync();
        }
        private void InitializeMembers()
        {
            operationManagement = new OperationManagement(client, credential);
            windowManagement = new WindowManagement(client, credential, Dispatcher);
            windowManagement.Run();
        }
        private void BindEvents()
        {
            Client.Connected += (s, e) => OnConnected();

            windowManagement.LoginFormVM.LoggingIn += (s, e) => OnLoggingIn(e);
            windowManagement.LoginFormVM.Registering += (s, e) => OnRegistering(e);

            windowManagement.FlatFormVM.FlatCreating = (s, e) => OnFlatCreating(e);
            windowManagement.FlatFormVM.FlatModifying = (s, e) => OnFlatModifying(e);

            windowManagement.HomeVM.QueryCreated += (s, e) => OnQueryCreated(e);

            operationManagement.FlatRegistered += (s, e) => windowManagement.OnResultReceived(e);
            operationManagement.FlatModificationRegistered += (s, e) => windowManagement.OnResultReceived(e);

            //operationManagement.QueryResultReceived +=(s,e)=>
            //operationManagement.PhotoReceived +=(s,e)=>wind
        }

        private void OnConnected()
        {
            Client.ReceiveAsync();
            OperationManagement.SendRequest(null, new Parameters()
            {
                Action = Act.Request,
                Direction = Direction.Realty,
                Initiator = Initiator.User,
                Target = Target.Lists
            });
        }
        private void OnLoggingIn(LoggingInEventArgs e)
        {
            Parameters parameters = new Parameters()
            {
                Direction = Direction.Identity,
                Action = Act.Login,
                Target = Target.Agent
            };
            operationManagement.SendRequest(e, parameters);
        }
        private void OnRegistering(RegisteringEventArgs e)
        {
            Parameters parameters = new Parameters()
            {
                Direction = Direction.Identity,
                Action = Act.Register,
                Target = Target.Agent
            };
            operationManagement.SendRequest(e, parameters);
        }

        private void OnQueryCreated(QueryCreatedEventArgs e)
        {
            Parameters parameters = new Parameters()
            {
                Direction = Direction.Realty,
                Action = Act.Request,
                Target = Target.Query
            };
            operationManagement.SendRequest(e, parameters);
        }
        private void OnFlatCreating(FlatCreatingEventArgs e)
        {
            Parameters parameters = new Parameters()
            {
                Direction = Direction.Realty,
                Action = Act.Add,
                Target = Target.Flat
            };
            operationManagement.SendRequest(e, parameters);
        }
        private void OnFlatModifying(FlatModifyingEventArgs e)
        {
            Parameters parameters = new Parameters()
            {
                Direction = Direction.Realty,
                Action = Act.Change,
                Target = Target.Flat
            };
            operationManagement.SendRequest(e.Flat, parameters);
        }
    }
}
