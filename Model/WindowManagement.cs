using RealtorObjects.Model.Event;
using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Event.RealtyEvents;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        #region Fileds and Properties
        private Credential credential;
        private Client client;

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
        #endregion

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
            loginForm = new LoginForm() { DataContext = loginFormVM };
            MainWindowVM = new MainWindowViewModel(credential);
            MainWindowVM.ViewModels[0] = HomeVM;
            MainWindowVM.WorkArea = HomeVM;
            mainWindow = new MainWindowV2 { DataContext = MainWindowVM };
        }
        private void BindEvents()
        {
            client.Disconnected += (s, e) => OnDisconnected();

            credential.LoggedIn += (s, e) => OnLoggedIn();
            credential.LoggedOut += (s, e) => OnLoggedOut();
            credential.Registered += (s, e) => OnRegistered();

            HomeVM.UpdateFinished += (s, e) => OnUpdateFinished();
            HomeVM.OpeningFlatForm += (s, e) => OnOpenFlatForm(e);

            FlatFormVM.FlatCreated = (s, e) => flatForm.Close();
        }

        private void OnDisconnected()
        {
            ((App)Application.Current).Shutdown();
            //mainWindow.Hide();
            //foreach (Window window in Application.Current.Windows)
            //    if (window is FlatFormV2 flatFormV2)
            //        flatFormV2.Close();
            //OpenLoadingForm();
            //Thread.Sleep(500);
            //client.ConnectAsync();
        }

        private void OnRegistered()
        {
            MessageBox.Show("Регистрация прошла успешно");
            loginFormVM.RegistrationVisibility = Visibility.Collapsed;
        }
        private void OnLoggedIn()
        {
            ((App)Application.Current).Dispatcher.Invoke((Action)delegate
            {
                loginForm.Close();
                loadingForm.Close();
                mainWindow.Show();
                MainWindowVM.CurrentAgentName = credential.Name;
            });
        }
        private void OnLoggedOut()
        {
            ((App)Application.Current).Dispatcher.Invoke((Action)delegate
            {
                mainWindow.Hide();
                loginForm = new LoginForm() { DataContext = loginFormVM };
                loginForm.Show();
            });
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
                    Type = TargetType.Flat,
                    Status = Status.Active
                };
            }
            else
            {
                flatFormVM.Title = "[Квартира] — Редактирование";
                flatFormVM.Flat = JsonSerializer.Deserialize<Flat>(JsonSerializer.Serialize(e.Flat)); //нужно, чтобы разорвать связь объекта в форме и объекта в списке
                flatFormVM.IsCurrentFlatNew = false;
            }
            if (e.LocationOptions != null)
                flatFormVM.LocationOptions = e.LocationOptions;
            else
                flatFormVM.LocationOptions = new LocationOptions();
            flatForm = new FlatFormV2 { DataContext = flatFormVM };
            flatForm.Show();
        }
        private void OnUpdateFinished()
        {
            Debug.WriteLine("Update is finished");
            ((App)Application.Current).Dispatcher.Invoke((Action)delegate
            {
                loadingForm.Close();
                loginForm.Show();
            });
        }
        public void OpenLoginForm()
        {
            loginForm.Show();
        }

        private void OpenLoadingForm()
        {
            ((App)Application.Current).Dispatcher.Invoke((Action)delegate
            {
                loadingForm = new LoadingForm() { DataContext = new LoadingFormViewModel() };
                loadingForm.Show();
            });
        }
    }
}
