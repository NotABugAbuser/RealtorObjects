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
        //КАНДИДАТЫ НА УДАЛЕНИЕ
        private void PassClientToViewModels()
        {
            //this.HomeVM.Client = this.Client;
            //this.MainWindowVM.Client = this.Client;
        }
        
        
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

            Client.ConnectAsync();
            OpenLoadingForm();
            operationManagement.AwaitOperationAsync();
        }
        private void InitializeMainMembers()
        {
            operationManagement = new OperationManagement(client, credential, Dispatcher);
            MainWindow = new MainWindowV2 { DataContext = MainWindowVM };
            MainWindowVM.ViewModels[0] = HomeVM;
            MainWindowVM.WorkArea = HomeVM;
        }
        private void BindEvents()
        {
            Client.Connected += () => { OpenLoginForm(); TestAutoLoginMeth(); };
            Client.LostConnection += () => Reconnect();

            credential.LoggedIn += (s, e) => OpenMainWindow();
            credential.LoggedOut += (s, e) => OpenLoginForm();
            credential.Registered += (s, e) => MessageBox.Show("Регистрация прошла успешно");
            
            operationManagement.UpdateFlat += (s, e) => HomeVM.RealtorObjectOperator.UpdateFlat(e.Flat);
            operationManagement.DeleteFlat += (s, e) => HomeVM.RealtorObjectOperator.DeleteFlat(e.Flat);
            //operationManagement.UpdateHouse += (s, e) => HomeVM.RealtorObjectOperator.UpdateFlat(e.Flat);
            //operationManagement.DeleteHouse += (s, e) => HomeVM.RealtorObjectOperator.DeleteFlat(e.Flat);
        }

        private void TestAutoLoginMeth()
        {
            credential.Name = "ГвоздиковЕА";
            credential.Password = "123";
            client.SendMessage(new Operation("ГвоздиковЕА", "123", OperationDirection.Identity, OperationType.Login));
        }
        private void OpenLoadingForm()
        {
            loadingForm = new LoadingForm() { DataContext = new LoadingFormViewModel() };
            loadingForm.Show();
            Thread.Sleep(1000);
        }
        private void OpenLoginForm()
        {
            loadingForm.Close();
            loginForm = new LoginForm() { DataContext = new LoginFormViewModel() };
            ((LoginFormViewModel)loginForm.DataContext).LoggingIn += (s, e) => operationManagement.Login(e.UserName, e.Password);
            loginForm.Show();
        }
        private void OpenMainWindow()
        {
            //if (loginForm.IsActive)
                loginForm.Close();
            //else if (loadingForm.IsActive)
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
        public Credential Credential { get => credential; private set => credential = value; }
        public OperationManagement OperationManagement { get => operationManagement; private set => operationManagement = value; }
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
