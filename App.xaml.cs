using RealtorObjects.Model;
using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Model;
using System;
using System.Diagnostics;
using System.Net;
using System.Windows;

namespace RealtorObjects
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            if (Debugger.IsAttached) {
                Client.ServerIp = IPAddress.Parse("192.168.56.1");
            } else {
                Client.ServerIp = IPAddress.Parse("192.168.1.250");
            }
            new LoginFormV2(new LoginFormVM()).Show();
        }
    }
}
