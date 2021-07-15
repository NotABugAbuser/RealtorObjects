using RealtorObjects.Model;
using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Model;
using System;
using System.Diagnostics;
using System.Windows;

namespace RealtorObjects
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            new LoginFormV2(new LoginFormVM()).Show();
        }
    }
}
