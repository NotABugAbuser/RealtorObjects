using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    class Worker : INotifyPropertyChanged
    {
        string agency = "";
        string registrant = "";
        DateTime regDate = new DateTime();
        string responsible = "";
        DateTime respDate = new DateTime();

        public string Agency {
            get => agency;
            set {
                agency = value;
                OnPropertyChanged();
            }
        }
        public string Registrant {
            get => registrant;
            set {
                registrant = value;
                OnPropertyChanged();
            }
        }
        public DateTime RegDate {
            get => regDate;
            set {
                regDate = value;
                OnPropertyChanged();
            }
        }
        public string Responsible {
            get => responsible;
            set {
                responsible = value;
                OnPropertyChanged();
            }
        }
        public DateTime RespDate {
            get => respDate;
            set {
                respDate = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
