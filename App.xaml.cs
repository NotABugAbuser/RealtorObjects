using RealtorObjects.View;
using RealtorObjects.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            var mainWindow = new MainWindow() { DataContext = new MainWindowViewModel() };
            var loginForm = new LoginForm() { DataContext = ((MainWindowViewModel)mainWindow.DataContext).ViewModels[0]};
            loginForm.Show();
        }
    }
}
