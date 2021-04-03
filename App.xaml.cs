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
        private Client client = new Client(Dispatcher.CurrentDispatcher);
        private FlatFormViewModel flatFormViewModel = new FlatFormViewModel();
        private LoginFormViewModel loginFormVM = new LoginFormViewModel();
        private MainWindowViewModel mainWindowVM = new MainWindowViewModel();
        private HomeViewModel homeVM = new HomeViewModel();
        private OperationManager operationManager;

        public Client Client
        {
            get => client;
            set => client = value;
        }
        public FlatFormViewModel FlatFormViewModel
        {
            get => flatFormViewModel;
            set => flatFormViewModel = value;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Client.ConnectAsync();
            MainWindow = new MainWindow();
            MainWindow.DataContext = MainWindowVM;
            MainWindowVM.ViewModels[0] = HomeVM;
            //MainWindowVM.WorkAreaViewModel = HomeVM;
            FlatFormViewModel.FlatCreated = HomeVM.HandleFlat;
            LoginFormVM.Logged += CloseLoginOpenMain;
            //operationManager = new OperationManager(Client);
            //operationManager.AwaitOperationAsync();
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
    }
}
