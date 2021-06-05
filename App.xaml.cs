using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Model;
using System;
using System.Windows;

namespace RealtorObjects
{
    public partial class App : Application
    {
        private Credential credential = new Credential();
        private String agentName = "";
        public String AgentName
        {
            get => agentName; set => agentName = value;
        }
        public Credential Credential
        {
            get => credential;
            private set => credential = value;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            new LoginForm(new LoginFormViewModel()).Show();
        }
    }
}
