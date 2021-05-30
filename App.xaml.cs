using RealtorObjects.Model;
using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Events.Identity;
using RealtyModel.Events.Realty;
using RealtyModel.Model;
using RealtyModel.Model.Operations;
using RealtyModel.Model.RealtyObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;

namespace RealtorObjects
{
    public partial class App : Application
    {
        #region Fields and Properties
        private Client client = new Client(Dispatcher.CurrentDispatcher);
        private Credential credential = new Credential();

        public Client Client
        {
            get => client;
            private set => client = value;
        }
        public Credential Credential
        {
            get => credential;
            private set => credential = value;
        }
        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            new LoginForm() {DataContext = new LoginFormViewModel() }.Show();
        }
    }
}
