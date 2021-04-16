using RealtorObjects.Model;
using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Event;
using RealtyModel.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RealtorObjects
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Client client = new Client(Dispatcher.CurrentDispatcher);
        private Credential credential = new Credential(Dispatcher.CurrentDispatcher);
        private RealtorObjectOperator realtorObjectOperator;
        private OperationManagement operationManagement;
        private WindowManagement windowManagement;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeMembers();
            BindEvents();

            Client.ConnectAsync();
            operationManagement.AwaitOperationAsync();
        }
        private void InitializeMembers()
        {
            operationManagement = new OperationManagement(client, credential, Dispatcher);
            windowManagement = new WindowManagement(client, credential);
            windowManagement.Run();
            realtorObjectOperator = new RealtorObjectOperator(windowManagement.HomeVM);
        }
        private void BindEvents()
        {
            windowManagement.LoginFormVM.LoggingIn += (s, e) => operationManagement.Login(e.UserName, e.Password);
            windowManagement.LoginFormVM.Registering += (s, e) => operationManagement.Register(e.UserName, e.Password, e.Email);
            windowManagement.FlatFormVM.FlatCreated = (s, e) => operationManagement.SendFlat(e.Flat, OperationType.Add);
            windowManagement.FlatFormVM.FlatModified = (s, e) => operationManagement.SendFlat(e.Flat, OperationType.Change);
            //windowManagement.HouseFormVM.HouseCreated = (s, e) => operationManagement.SendFlat(e.House, OperationType.Add);

            //operationManagement.ReceivedLists +=(s,e)=> отправить списки
            //operationManagement.ReceivedObjectDB +=(s,e)=> отправить объекты
            operationManagement.UpdateFlat += (s, e) => RealtorObjectOperator.UpdateFlat(e.Flat);
            operationManagement.DeleteFlat += (s, e) => RealtorObjectOperator.DeleteFlat(e.Flat);
            operationManagement.UpdateHouse += (s, e) => RealtorObjectOperator.UpdateHouse(e.House);
            operationManagement.DeleteHouse += (s, e) => RealtorObjectOperator.UpdateHouse(e.House);
        }

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
        public RealtorObjectOperator RealtorObjectOperator
        {
            get => realtorObjectOperator;
            set => realtorObjectOperator = value;
        }
    }
}
