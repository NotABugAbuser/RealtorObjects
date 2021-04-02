using RealtorObjects.Model;
using RealtorObjects.View;
using RealtorObjects.ViewModel;
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
        public Client Client {
            get => client;
            set => client = value;
        }
        public FlatFormViewModel FlatFormViewModel {
            get => flatFormViewModel; set => flatFormViewModel = value;
        }

        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            Client.ConnectAsync();

            for (byte attempts = 0; attempts <= 30; attempts++) {
                if (Client.IsConnected) {
                    var mainWindow = new MainWindow() { DataContext = new MainWindowViewModel() };
                    var loginForm = new LoginForm() { DataContext = ((MainWindowViewModel)mainWindow.DataContext).ViewModels[0] };
                    loginForm.Show();
                    //mainWindow.Show();
                    break;
                }
                if (attempts == 30) {
                    MessageBox.Show("Сервер недоступен");
                    Application.Current.Shutdown();
                }
                Thread.Sleep(100);
            }
        }
    }
}
