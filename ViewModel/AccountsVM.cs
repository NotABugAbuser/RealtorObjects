using RealtorObjects.Model;
using RealtyModel.Model;
using RealtyModel.Model.Operations;
using RealtyModel.Service;
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
        private ObservableCollection<Credential> credentials = new ObservableCollection<Credential>();
        private CustomCommand cancel;
        private CustomCommand confirm;
        private CustomCommand add;
        private CustomCommand delete;
        public AccountsVM() {
        }
        public AccountsVM(List<Credential> credentials) {
            Credentials = new ObservableCollection<Credential>(credentials);
        }
        public CustomCommand Delete => delete ?? (delete = new CustomCommand(obj => {
            Credentials.Remove(obj as Credential);
        }));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj => {
            bool isEveryFieldFilled = true;
            foreach (Credential c in Credentials) {
                if (String.IsNullOrEmpty(c.Name) || String.IsNullOrEmpty(c.Password)) {
                    isEveryFieldFilled = false;
                    break;
                }
            }
            if (isEveryFieldFilled) {
                Client.UpdateCredentials(Credentials.ToList());
                (obj as Window).Close();
            } else {
                OperationNotification.Notify(ErrorCode.NotFilled);
            }
        }));
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj => {
            (obj as Window).Close();
        }));
        public CustomCommand Add => add ?? (add = new CustomCommand(obj => {
            Credential credential = new Credential();
            credential.RegistrationDate = DateTime.Now;
            credential.Id = credentials.Max(cr => cr.Id) + 1;
            Credentials.Add(credential);
        }));
        public ObservableCollection<Credential> Credentials {
            get => credentials;
            set {
                credentials = value;
                OnPropertyChanged();
            }
        }
    }
}
