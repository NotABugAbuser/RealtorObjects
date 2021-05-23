using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Events.Realty;
using RealtyModel.Events.UI;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;
using RealtyModel.Model.RealtyObjects;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace RealtorObjects.Model
{
    public class WindowManagement
    {
        #region Fileds and Properties
        private Client client;
        private Credential credential;
        private Dispatcher dispatcher;

        private Window mainWindow;
        private LoginForm loginForm;
        private LoadingForm loadingForm;
        private FlatFormV2 flatForm;

        private HomeViewModel homeVM;
        private FlatFormViewModel flatFormVM;
        private HouseFormViewModel houseFormVM;
        private LoginFormViewModel loginFormVM;
        private MainWindowViewModel mainWindowVM;

        public HomeViewModel HomeVM
        {
            get => homeVM;
            private set => homeVM = value;
        }
        public FlatFormViewModel FlatFormVM
        {
            get => flatFormVM;
            set => flatFormVM = value;
        }
        public HouseFormViewModel HouseFormVM
        {
            get => houseFormVM;
            set => houseFormVM = value;
        }
        public LoginFormViewModel LoginFormVM
        {
            get => loginFormVM;
            set => loginFormVM = value;
        }
        public MainWindowViewModel MainWindowVM
        {
            get => mainWindowVM;
            private set => mainWindowVM = value;
        }
        public RealtyManagement RealtyManagement { get; set; }
        #endregion

        public WindowManagement(Client client, Credential credential, Dispatcher dispatcher)
        {
            this.client = client;
            this.credential = credential;
            this.dispatcher = dispatcher;
        }

        internal void Run()
        {
            InitializeMembers();
            BindEvents();
        }
        private void InitializeMembers()
        {
            mainWindow = ((App)Application.Current).MainWindow;
            HomeVM = new HomeViewModel();
            HomeVM.DispatcherTest = this.dispatcher;
            flatFormVM = new FlatFormViewModel();
            loginFormVM = new LoginFormViewModel();
            loginForm = new LoginForm() { DataContext = loginFormVM };
            MainWindowVM = new MainWindowViewModel(credential);
            MainWindowVM.ViewModels[0] = HomeVM;
            MainWindowVM.WorkArea = HomeVM;
            mainWindow = new MainWindowV2 { DataContext = MainWindowVM };
        }
        private void BindEvents()
        {
            client.Connecting += (s, e) => OpenLoadingForm();
            client.Connected += (s, e) => loadingForm.Close();
            client.Connected += (s, e) => loginForm.Show();
            client.Disconnected += (s, e) => OnDisconnected();

            credential.LoggedIn += (s, e) => OnLoggedIn();
            credential.LoggedOut += (s, e) => OnLoggedOut();
            credential.Registered += (s, e) => OnRegistered();
            HomeVM.FlatButtonPressed += (s, e) => OpenFlatForm(e);
        }


        private void OnDisconnected()
        {
            if (!Debugger.IsAttached)
                ((App)Application.Current).Shutdown();
            else
            {
                loadingForm.Close();
                mainWindow.Hide();
                flatForm?.Hide();
                while (client.IsConnected) { }
                client.ConnectAsync();
            }
        }
        internal void OnResultReceived(FlatRegisteredEventArgs e)
        {
            MessageBox.Show($"Квартира добавлена");
            CloseFlatForm();
        }
        internal void OnResultReceived(FlatModificationRegisteredEventArgs e)
        {
            MessageBox.Show($"Квартира изменена");
            CloseFlatForm();
        }

        private void OnRegistered()
        {
            MessageBox.Show("Регистрация прошла успешно");
            loginFormVM.RegistrationVisibility = Visibility.Collapsed;
        }
        private void OnLoggedIn()
        {
            dispatcher.Invoke(() =>
            {
                loginForm.Close();
                if (flatForm != null && flatForm.IsInitialized)
                    flatForm.Show();
                mainWindow.Show();
                MainWindowVM.CurrentAgentName = credential.Name;
            });
        }
        private void OnLoggedOut()
        {
            dispatcher.Invoke(() =>
            {
                mainWindow.Hide();
                loginForm = new LoginForm() { DataContext = loginFormVM };
                loginForm.Show();
            });
        }

        public void OnListsArrived(ListsArrivedEventArgs e)
        {
            HomeVM.LocationOptions = e.LocationOptions;
        }
        internal void OpenFlatForm(FlatButtonPressedEventArgs e)
        {
            if (e.IsNewFlat)
            {
                flatFormVM.Title = "[Квартира] — Создание";
                flatFormVM.IsCurrentFlatNew = true;
                flatFormVM.Flat = new Flat()
                {
                    Agent = credential.Name,
                    Album = new Album()
                    {
                        Location = "sdsa",
                        PhotoCollection = new ObservableCollection<byte[]>()
                    },
                    Location = new Location()
                    {
                        City = new City() { Name = "asd" },
                        District = new District() { Name = "asd" },
                        Street = new Street() { Name = "asd" },
                        HouseNumber = 1,
                        FlatNumber = 1,
                        HasBanner = false,
                        HasExchange = false
                    },
                    Cost = new Cost()
                    {
                        Area = 10,
                        HasMortgage = false,
                        HasPercents = false,
                        HasVAT = false,
                        Price = 1220
                    },
                    Customer = new Customer()
                    {
                        Name = "asd",
                        PhoneNumbers = "123213"
                    },
                    GeneralInfo = new BaseInfo()
                    {
                        Ceiling = 10,
                        Condition = "asdsa",
                        Convenience = "asd",
                        Description = "asd",
                        General = 10,
                        Heating = "asd",
                        Kitchen = 10,
                        Living = 10,
                        RoomCount = 10,
                        Water = "asdsad",
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
                        Kvl = 10,
                        Loggia = "asd",
                        Material = "asd",
                        Rooms = "asdsad",
                        Type = "asd",
                        TypeOfRooms = "asdsa",
                        Windows = "asdsad"
                    },
                    HasExclusive = false,
                    IsSold = false,
                    Type = Target.Flat,
                    Status = Status.Active
                };
                Flat flat = new Flat()
                {
                    Agent = credential.Name,
                    Album = new Album()
                    {
                        Location = "",
                        PhotoCollection = new ObservableCollection<Byte[]>()
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
                        Balcony = "",
                        Bath = "",
                        Bathroom = "",
                        Floor = "",
                        Fund = "",
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
                        Windows = ""
                    },
                    HasExclusive = false,
                    IsSold = false,
                    Type = Target.Flat,
                    Status = Status.Active
                };
            }
            else
            {
                flatFormVM.Title = "[Квартира] — Редактирование";
                flatFormVM.Flat = JsonSerializer.Deserialize<Flat>(JsonSerializer.Serialize(e.Flat)); //нужно, чтобы разорвать связь объекта в форме и объекта в списке
                flatFormVM.IsCurrentFlatNew = false;
            }
            flatFormVM.LocationOptions = this.HomeVM.LocationOptions;
            flatForm = new FlatFormV2 { DataContext = flatFormVM };
            flatForm.Show();
        }
        internal void CloseFlatForm()
        {
            dispatcher.Invoke(() => { flatForm.Close(); });
        }
        private void OpenLoadingForm()
        {
            dispatcher.Invoke(() =>
            {
                loadingForm = new LoadingForm() { DataContext = new LoadingFormViewModel() };
                loadingForm.Show();
            });
        }
    }
}
