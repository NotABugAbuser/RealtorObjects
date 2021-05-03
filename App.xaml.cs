using RealtorObjects.Model;
using RealtyModel.Model;
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
            Client.Connected += () => Client.CheckConnectionAsync();
            Client.Connected += () => Client.ReceiveOverStreamAsync();

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
