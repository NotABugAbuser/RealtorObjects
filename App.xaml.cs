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
        private FlatFormViewModel flatFormVM = new FlatFormViewModel();
        private LoginFormViewModel loginFormVM = new LoginFormViewModel();
        private MainWindowViewModel mainWindowVM = new MainWindowViewModel();
        private HomeViewModel homeVM = new HomeViewModel();
        private Client client = new Client(Dispatcher.CurrentDispatcher);


        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            InitializeMainMembers();
            PassClientToViewModels();
            TryConnectToServer();

        }
        private void PassClientToViewModels() {
            this.FlatFormVM.Client = this.Client;
            this.HomeVM.Client = this.Client;
            this.MainWindowVM.Client = this.Client;
            this.LoginFormVM.Client = this.Client;
        }
        private void InitializeMainMembers() {
            this.MainWindow = new MainWindowV2 { DataContext = MainWindowVM };
            this.MainWindowVM.ViewModels[0] = HomeVM;
            this.MainWindowVM.WorkArea = HomeVM;
            this.LoginFormVM.Logged += CloseLoginOpenMain;

            this.FlatFormVM.FlatCreated = HomeVM.HandleFlat; //вариант для тестов и отладки
            //this.FlatFormVM.FlatCreated = HomeVM.RealtorObjectHandler.HandleFlat; //правильный вариант

        }
        private void TryConnectToServer() {
            Client.ConnectAsync();
            for (byte attempts = 0; attempts <= 30; attempts++) {
                if (Client.IsConnected) {
                    var loginForm = new LoginForm() { DataContext = LoginFormVM };
                    loginForm.Show();
                    break;
                }
                if (attempts == 30) {
                    MessageBox.Show("Сервер недоступен");
                    Application.Current.Shutdown();
                }
                Thread.Sleep(100);
            }
        }

        private void CloseLoginOpenMain(object sender, LoggedEventArgs e) {
            ((Window)e.Window).Close();
            MainWindow.Show();
        }
        public Client Client {
            get => client;
            private set => client = value;
        }
        public FlatFormViewModel FlatFormVM {
            get => flatFormVM;
            private set => flatFormVM = value;
        }
        public LoginFormViewModel LoginFormVM {
            get => loginFormVM;
            private set => loginFormVM = value;
        }
        public MainWindowViewModel MainWindowVM {
            get => mainWindowVM;
            private set => mainWindowVM = value;
        }
        public HomeViewModel HomeVM {
            get => homeVM;
            private set => homeVM = value;
        }
    }
}
