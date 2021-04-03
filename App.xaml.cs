using RealtorObjects.Model;
using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Event;
using RealtyModel.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        private FlatFormViewModel flatFormVM;
        private LoginFormViewModel loginFormVM;
        private MainWindowViewModel mainWindowVM;
        private HomeViewModel homeVM;
        private OperationHandler operationManager;
        private Client client = new Client(Dispatcher.CurrentDispatcher);


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeViewModels();
            TryConnectToServer();
            InitializeMainMembers();

        }
        private void InitializeViewModels()
        {
            //Вььюмодели инициализируются здесь, чтобы они могли получить
            //экземлпяр клиента из приложения через конструкторы
            this.FlatFormVM = new FlatFormViewModel();
            this.LoginFormVM = new LoginFormViewModel();
            this.MainWindowVM = new MainWindowViewModel();
            this.HomeVM = new HomeViewModel();
        }
        private void InitializeMainMembers()
        {
            this.MainWindow = new MainWindow
            {
                DataContext = MainWindowVM
            };
            this.MainWindowVM.ViewModels[0] = HomeVM;
            this.MainWindowVM.WorkAreaViewModel = HomeVM;
            this.FlatFormVM.FlatCreated = HomeVM.HandleFlat;
            this.LoginFormVM.Logged += CloseLoginOpenMain;
            //operationManager = new OperationManager(Client);
            //operationManager.AwaitOperationAsync();
        }
        private void TryConnectToServer()
        {
            Client.ConnectAsync();
            for (byte attempts = 0; attempts <= 30; attempts++)
            {
                if (Client.IsConnected)
                {
                    var loginForm = new LoginForm() { DataContext = LoginFormVM };
                    loginForm.Show();
                    break;
                }
                if (attempts == 30)
                {
                    MessageBox.Show("Сервер недоступен");
                    Application.Current.Shutdown();
                }
                Thread.Sleep(100);
            }
        }

        private void CloseLoginOpenMain(object sender, LoggedEventArgs e)
        {
            ((Window)e.Window).Close();
            MainWindow.Show();
        }
        public Client Client
        {
            get => client;
            set => client = value;
        }
        public FlatFormViewModel FlatFormVM
        {
            get => flatFormVM;
            set => flatFormVM = value;
        }
        public LoginFormViewModel LoginFormVM
        {
            get => loginFormVM;
            set => loginFormVM = value;
        }
        public MainWindowViewModel MainWindowVM
        {
            get => mainWindowVM;
            set => mainWindowVM = value;
        }
        public HomeViewModel HomeVM
        {
            get => homeVM;
            set => homeVM = value;
        }
    }
}
