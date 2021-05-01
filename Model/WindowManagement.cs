using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Event.RealtyEvents;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.Model
{
    public class WindowManagement
    {
        private Credential credential;
        private Client client;
        private Window mainWindow;
        private LoginForm loginForm;
        private LoadingForm loadingForm;

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


        public WindowManagement()
        {

        }
        public WindowManagement(Client client, Credential credential)
        {
            this.client = client;
            this.credential = credential;
        }

        public void Run()
        {
            InitializeMembers();
            BindEvents();
            OpenLoadingForm();
        }
        private void InitializeMembers()
        {
            mainWindow = ((App)Application.Current).MainWindow;
            HomeVM = new HomeViewModel();
            flatFormVM = new FlatFormViewModel();
            loginFormVM = new LoginFormViewModel();
            MainWindowVM = new MainWindowViewModel();
            MainWindowVM.ViewModels[0] = HomeVM;
            MainWindowVM.WorkArea = HomeVM;
            mainWindow = new MainWindowV2 { DataContext = MainWindowVM };
        }
        private void BindEvents()
        {
            client.Connected += () => HomeVM.GetUpdate();
            HomeVM.UpdateFinished += (s, e) => OpenLoginForm();
            client.LostConnection += () => Reconnect();
            credential.LoggedIn += (s, e) => OpenMainWindow();
            credential.LoggedOut += (s, e) => OpenLoginForm();
            credential.Registered += (s, e) =>
            {
                MessageBox.Show("Регистрация прошла успешно");
                loginFormVM.RegistrationVisibility = Visibility.Collapsed;
            };
            HomeVM.OpeningFlatForm += (s, e) => OnOpenFlatForm(e);
        }

        private void TestAutoLoginMeth()
        {
            ((App)Application.Current).Credential.Name = "ГвоздиковЕА";
            ((App)Application.Current).Credential.Password = "123";
            client.OutcomingOperations.Enqueue(new Operation("ГвоздиковЕА", "123", OperationDirection.Identity, OperationType.Login));
        }
        private void OpenLoadingForm()
        {
            ((App)Application.Current).Dispatcher.Invoke((Action)delegate
            {
                loadingForm = new LoadingForm() { DataContext = new LoadingFormViewModel() };
                loadingForm.Show();
                //Thread.Sleep(1000);
            });
        }
        private void OpenLoginForm()
        {
            ((App)Application.Current).Dispatcher.Invoke((Action)delegate
            {
                loadingForm.Close();
                loginForm = new LoginForm() { DataContext = loginFormVM };
                loginForm.Show();
                TestAutoLoginMeth();
                Thread.Sleep(1000);
            });
        }
        private void OpenMainWindow()
        {
            ((App)Application.Current).Dispatcher.Invoke((Action)delegate
            {
                loginForm.Close();
                loadingForm.Close();
                client.Connected += () => OpenMainWindow();
                HomeVM.FilterCollection?.Execute(new object());
                mainWindow.Show();
            });
        }
        private void Reconnect()
        {
            mainWindow.Hide();
            OpenLoadingForm();
            var timer = new System.Timers.Timer(1000);
            timer.AutoReset = false;
            timer.Elapsed += (s, e) =>
            {
                client.ConnectAsync();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        public void OnOpenFlatForm(OpeningFlatFormEventArgs e)
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
                        Location = "",
                        PhotoList = new byte[0],
                        Preview = new byte[0],
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
                    ObjectType = "",
                    Status = Status.Active
                };
            }
            else
            {
                flatFormVM.Title = "[Квартира] — Редактирование";
                flatFormVM.Flat = JsonSerializer.Deserialize<Flat>(JsonSerializer.Serialize(e.Flat)); //нужно, чтобы разорвать связь объекта в форме и объекта в списке
                flatFormVM.IsCurrentFlatNew = false;
            }
            flatFormVM.LocationOptions = new LocationOptions();
            new FlatFormV2 { DataContext = flatFormVM }.Show();
        }
    }
}
