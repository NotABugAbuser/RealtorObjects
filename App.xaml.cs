using RealtorObjects.Model;
using RealtyModel.Events.Identity;
using RealtyModel.Events.Realty;
using RealtyModel.Events.UI;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;
using RealtyModel.Model.RealtyObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
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
        public List<Photo> UnsavedPhotos { get; set; }
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
            UnsavedPhotos = new List<Photo>();
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
            windowManagement.FlatFormVM.FlatChanging = (s, e) => OnFlatChanging(e);

            operationManagement.ReceivedFlat += (s, e) => windowManagement.OnReceivedFlat(e);
            operationManagement.ReceivedPhoto += (s, e) => OnReceivedPhoto(e);
        }

        private void OnConnected()
        {
            Client.CheckConnectionAsync();
            Client.ReceiveAsync();
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

        private void OnFlatCreating(FlatCreatingEventArgs e)
        {
            Parameters parameters = new Parameters()
            {
                Direction = Direction.Realty,
                Action = Act.Add,
                Target = Target.Flat
            };
            if (e.Flat.Album.PhotoCollection != null && e.Flat.Album.PhotoCollection.Count > 0)
                foreach (Byte[] data in e.Flat.Album.PhotoCollection)
                    UnsavedPhotos.Add(new Photo() { Location = e.Flat.Album.Location, ObjectType = Target.Flat, Data = data, Guid = (Guid.NewGuid()).ToString() });
            operationManagement.SendRequest(e, parameters);
        }
        private void OnFlatChanging(FlatChangingEventArgs e)
        {
            Parameters parameters = new Parameters()
            {
                Direction = Direction.Realty,
                Action = Act.Change,
                Target = Target.Flat
            };
            operationManagement.SendRequest(e.Flat, parameters);
        }
        private void OnReceivedPhoto(ReceivedPhotoEventArgs e)
        {
            if (e.Act == Act.Add)
            {
                Photo photo = UnsavedPhotos.Find(ph => ph.Guid == e.Data);
                if (photo != null)
                    UnsavedPhotos.Remove(photo);
            }
            else if (e.Act == Act.Request)
            {
            }
        }
    }
}
