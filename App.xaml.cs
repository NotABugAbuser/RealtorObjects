using RealtorObjects.View;
using RealtorObjects.ViewModel;
using RealtyModel.Model;
using System.Windows;

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
