using RealtorObjects.Model;
using RealtyModel.Model;
using RealtyModel.Model.Operations;
using RealtyModel.Model.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    public class AccountsVM : BaseVM
    {
        private ObservableCollection<Agent> agents = new ObservableCollection<Agent>();
        private CustomCommand cancel;
        private CustomCommand confirm;
        private CustomCommand add;
        private CustomCommand delete;
        public AccountsVM() {
        }
        public AccountsVM(List<Agent> agents) {
            Agents = new ObservableCollection<Agent>(agents);
        }
        public CustomCommand Delete => delete ?? (delete = new CustomCommand(obj => {
            Agents.Remove(obj as Agent);
        }));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj => {
            bool isEveryFieldFilled = true;
            foreach (Agent c in Agents) {
                if (String.IsNullOrEmpty(c.Name) 
                || String.IsNullOrEmpty(c.Surname)
                || String.IsNullOrEmpty(c.Patronymic)
                || String.IsNullOrEmpty(c.Password)
                || String.IsNullOrEmpty(c.Email)
                || String.IsNullOrEmpty(c.PhoneNumber)) {
                    isEveryFieldFilled = false;
                    break;
                }
            }
            if (isEveryFieldFilled && Client.CanConnect()) {
                Debug.WriteLine(Agents == null);
                Debug.WriteLine(Agents.Count);
                Client.UpdateAgents(Agents.ToList());
                (obj as Window).Close();
            } else {
                OperationNotification.Notify(ErrorCode.NotFilled);
            }
        }));
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj => {
            (obj as Window).Close();
        }));
        public CustomCommand Add => add ?? (add = new CustomCommand(obj => {
            Agent agent = new Agent();
            agent.Id = Agents.Max(a => a.Id) + 1;
            agent.RegistrationDate = DateTime.Now;
            Agents.Add(agent);
        }));
        public ObservableCollection<Agent> Agents {
            get => agents;
            set {
                agents = value;
                OnPropertyChanged();
            }
        }
    }
}
