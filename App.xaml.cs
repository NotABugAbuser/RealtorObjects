using RealtorObjects.Model;
using RealtorObjects.View;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
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
        private RealtyManagement realtyManagement;

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
        public RealtyManagement RealtyManagement
        {
            get => realtyManagement;
            set => realtyManagement = value;
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
            realtyManagement = new RealtyManagement(Dispatcher);
            operationManagement = new OperationManagement(client, credential, Dispatcher);
            windowManagement = new WindowManagement(client, credential, Dispatcher);
            windowManagement.Run();
        }
        private void BindEvents()
        {
            Client.Connected += (s, e) => Client.CheckConnectionAsync();
            Client.Connected += (s, e) => Client.ReceiveAsync();
            Client.Connected += (s, e) => CheckClientStatus();

            windowManagement.LoginFormVM.LoggingIn += (s, e) => operationManagement.SendIdentityData(e.UserName, e.Password, OperationType.Login);
            windowManagement.LoginFormVM.Registering += (s, e) => operationManagement.SendIdentityData(e.UserName, $"{e.Password};{e.Email}", OperationType.Register);
            windowManagement.FlatFormVM.FlatCreated = (s, e) => OnFlatCreated(e.Flat);
            windowManagement.FlatFormVM.FlatModified = (s, e) => operationManagement.SendRealtyData(e.Flat, OperationType.Change, TargetType.Flat);

            operationManagement.ReceivedDbUpdate += (s, e) => realtyManagement.ReceiveDbUpdate(e);

            operationManagement.ReceivedFlat += (s, e) =>
            {
                if (realtyManagement.AddFlat(e.Flat) && e.Flat.Location.CompareWith(windowManagement.FlatFormVM.CurrentLocation))
                {
                    Dispatcher.Invoke(() =>
                    {
                        foreach(Photo photo in UnsavedPhotos)
                            if (photo.Location == e.Flat.Album.Location)
                                operationManagement.SendRealtyData(photo, OperationType.Add, TargetType.Photo);
                        windowManagement.HomeVM.AllObjects.Add(e.Flat);
                        windowManagement.HomeVM.FilterCollection.Execute(new object());
                        windowManagement.CloseFlatForm();
                    });
                }
            };
            operationManagement.ReceivedFlatUpdate += (s, e) =>
            {
                if (realtyManagement.UpdateFlat(e.Flat) && e.Flat.Location.CompareWith(windowManagement.FlatFormVM.CurrentLocation))
                {
                    Flat listFlat = (Flat)(windowManagement.HomeVM).AllObjects.Find(f => f.Id == e.Flat.Id && f.Type == e.Flat.Type);
                    listFlat = e.Flat;
                    windowManagement.HomeVM.FilterCollection.Execute(new object());
                    windowManagement.CloseFlatForm();
                }
            };
            operationManagement.ReceivedFlatDeletion += (s, e) =>
            {
                if (realtyManagement.DeleteFlat(e.Flat))
                {
                    Flat listFlat = (Flat)(windowManagement.HomeVM).AllObjects.Find(f => f.Id == e.Flat.Id && f.Type == e.Flat.Type);
                    windowManagement.HomeVM.AllObjects.Remove(listFlat);
                    windowManagement.HomeVM.FilterCollection.Execute(new object());
                }
            };
            //ЗДЕСЬ НУЖНО ДОДЕЛАТЬ
            operationManagement.ReceivedPhoto += (s, e) =>
            {
                if (e.OperationType == OperationType.Add)
                {
                    Guid guid = JsonSerializer.Deserialize<Guid>(Encoding.UTF8.GetString(e.Data));
                    Photo photo = UnsavedPhotos.Find(ph => ph.Guid == guid);
                    if (photo != null)
                        UnsavedPhotos.Remove(photo);
                }
                //ЧТО ДАЛЬШЕ?
                else if (e.OperationType == OperationType.Get)
                {
                    Photo photo = JsonSerializer.Deserialize<Photo>(Encoding.UTF8.GetString(e.Data));
                }
            };

            operationManagement.ReceivedHouse += (s, e) => realtyManagement.AddHouse(e.House);
            operationManagement.ReceivedHouseUpdate += (s, e) => realtyManagement.UpdateHouse(e.House);
            operationManagement.ReceivedHouseDeletion += (s, e) => realtyManagement.DeleteHouse(e.House);

            realtyManagement.UpdateFinished += (s, e) => windowManagement.OnUpdateFinished();
        }

        private void OnFlatCreated(Flat flat)
        {
            operationManagement.SendRealtyData(flat, OperationType.Add, TargetType.Flat);
            foreach (Byte[] data in flat.Album.PhotoCollection)
                UnsavedPhotos.Add(new Photo() { Location = flat.Album.Location, ObjectType = TargetType.Flat, Data = data, Guid = Guid.NewGuid()});
        }

        private void CheckClientStatus()
        {
            //Если в режиме дебага - автологин
            //if (Debugger.IsAttached)
            //    operationManagement.SendIdentityData("ГриньДВ", "123", OperationType.Login);
            //else
            //{
            //Если первый раз подключился за запуск программы и не имеет обновления
            if (Client.IsFirstConnection || !realtyManagement.HasUpdate)
                operationManagement.SendRealtyData(null, OperationType.Update, TargetType.All);
            //Если был залогинен и credential дынные не пустые
            else if (Credential.IsLoggedIn && !String.IsNullOrWhiteSpace(Credential.Name) && !String.IsNullOrWhiteSpace(Credential.Password))
                operationManagement.SendIdentityData(Credential.Name, Credential.Password, OperationType.Login);
            //иначе открыть LoginForm
            else windowManagement.OpenLoginForm();
            operationManagement.SendIdentityData("ГриньДВ", "123", OperationType.Login);
            //}
        }


        private void Test()
        {
            Flat flat = new Flat()
            {
                Agent = "asdasd",
                Album = new Album()
                {
                    Location = "asdas",
                },
                Location = new Location()
                {
                    City = new City(),
                    District = new District(),
                    Street = new Street(),
                    HouseNumber = 0,
                    FlatNumber = 0,
                    HasBanner = false,
                    HasExchange = false
                },
                Cost = new Cost()
                {
                    Area = 0,
                    HasMortgage = false,
                    HasPercents = false,
                    HasVAT = false,
                    Price = 0
                },
                Customer = new Customer()
                {
                    Name = "",
                    PhoneNumbers = ""
                },
                GeneralInfo = new BaseInfo()
                {
                    Ceiling = 0,
                    Condition = "",
                    Convenience = "",
                    Description = "",
                    General = 0,
                    Heating = "",
                    Kitchen = 0,
                    Living = 0,
                    RoomCount = 0,
                    Water = "",
                    Year = 1950
                },
                Info = new FlatInfo()
                {
                    Balcony = "asd",
                    Bath = "asd",
                    Bathroom = "asd",
                    Floor = "asd",
                    Fund = "asd",
                    HasChute = false,
                    HasElevator = false,
                    HasGarage = false,
                    HasImprovedLayout = false,
                    HasRenovation = false,
                    IsCorner = false,
                    IsPrivatised = false,
                    IsSeparated = false,
                    Kvl = 0,
                    Loggia = "",
                    Material = "",
                    Rooms = "",
                    Type = "",
                    TypeOfRooms = "",
                    Windows = "Linux"
                },
                HasExclusive = false,
                IsSold = false,
                Type = TargetType.Flat,
                Status = Status.Active
            };
            String s = JsonSerializer.Serialize((BaseRealtorObject)flat);
            Byte[] d = Encoding.UTF8.GetBytes(s);
            s = Encoding.UTF8.GetString(d);
            flat = JsonSerializer.Deserialize<Flat>(s);
            Debug.WriteLine($"\n\n{flat.Info.Windows}\n\n");
        }
    }
}
