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
        private FlatFormViewModel flatFormVM = new FlatFormViewModel();
        private LoginFormViewModel loginFormVM = new LoginFormViewModel();
        private MainWindowViewModel mainWindowVM = new MainWindowViewModel();
        private HomeViewModel homeVM = new HomeViewModel();
        private Credential credential = new Credential(Dispatcher.CurrentDispatcher);
        private Client client = new Client(Dispatcher.CurrentDispatcher);
        private OperationManagement operationManagement = new OperationManagement();
        private LoginForm loginForm = null;
        private LoadingForm loadingForm = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitializeMainMembers();
            BindEvents();
            PassClientToViewModels();
            Client.ConnectAsync();
            OpenLoadingForm();
            operationManagement.AwaitOperationAsync();
        }
        private void PassClientToViewModels()
        {
            this.FlatFormVM.Client = this.Client;
            this.HomeVM.Client = this.Client;
            this.MainWindowVM.Client = this.Client;
            this.LoginFormVM.Client = this.Client;
        }
        private void InitializeMainMembers()
        {
            operationManagement = new OperationManagement(client, credential, Dispatcher);
            //вариант для тестов и отладки
            //this.FlatFormVM.FlatCreated = HomeVM.RealtorObjectHandler.HandleFlat; //правильный вариант
            MainWindow = new MainWindowV2 { DataContext = MainWindowVM };
            MainWindowVM.ViewModels[0] = HomeVM;
            MainWindowVM.WorkArea = HomeVM;
        }
        private void BindEvents()
        {
            FlatFormVM.FlatCreated = HomeVM.HandleFlat;
            Client.Connected += () => { OpenLoginForm(); TestAutoLoginMeth(); };
            Client.LostConnection += () => Reconnect();
            LoginFormVM.Logging += (s, e) => operationManagement.Login(e.UserName, e.Password);
            credential.LoggedIn += () => OpenMainWindow();
            operationManagement.UpdateFlat += (s, e) => HomeVM.RealtorObjectOperator.UpdateFlat(e.Flat);
            operationManagement.DeleteFlat += (s, e) => HomeVM.RealtorObjectOperator.DeleteFlat(e.Flat);
        }

        private void TestAutoLoginMeth()
        {
            credential.Name = "ГриньДВ";
            credential.Password = "123";
            client.SendMessage(new Operation("ГриньДВ", "123", OperationDirection.Identity, OperationType.Login));
        }
        private void OpenLoadingForm()
        {
            loadingForm = new LoadingForm() { DataContext = new LoadingFormViewModel() };
            loadingForm.Show();
        }
        private void OpenLoginForm()
        {
            loadingForm.Close();
            loginForm = new LoginForm() { DataContext = LoginFormVM };
            loginForm.Show();
        }
        private void OpenMainWindow()
        {
            if (loginForm.IsActive)
                loginForm.Close();
            else if (loadingForm.IsActive)
                loadingForm.Close();
            Client.Connected += () => OpenMainWindow();
            MainWindow.Show();
        }
        private void Reconnect()
        {
            MainWindow.Hide();
            OpenLoadingForm();
            var timer = new System.Timers.Timer(1000);
            timer.AutoReset = false;
            timer.Elapsed += (s, e) =>
            {
                Client.ConnectAsync();
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        public Client Client
        {
            get => client;
            private set => client = value;
        }
        public FlatFormViewModel FlatFormVM
        {
            get => flatFormVM;
            private set => flatFormVM = value;
        }
        public LoginFormViewModel LoginFormVM
        {
            get => loginFormVM;
            private set => loginFormVM = value;
        }
        public MainWindowViewModel MainWindowVM
        {
            get => mainWindowVM;
            private set => mainWindowVM = value;
        }
        public HomeViewModel HomeVM
        {
            get => homeVM;
            private set => homeVM = value;
        }
    }
}
