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
        private Credential credential = new Credential();
        private string agentName = "";
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            new LoginForm(new LoginFormViewModel()).Show();
        }
        public Credential Credential {
            get => credential;
            private set => credential = value;
        }
        public string AgentName {
            get => agentName; set => agentName = value;
        }
    }
}
