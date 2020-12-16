using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    public class PersonInfo : INotifyPropertyChanged
    {
        string agency = "";
        string registrant = "";
        DateTime regDate = new DateTime();
        string responsible = "";
        DateTime respDate = new DateTime();
        bool hasExclusive = false;
        string exclusiveNumber = "";
        string phoneNumbers = "";
        string customer = "";


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

        public bool HasExclusive {
            get => hasExclusive;
            set {
                hasExclusive = value;
                OnPropertyChanged();
            }
        }
        public string ExclusiveNumber {
            get => exclusiveNumber;
            set {
                exclusiveNumber = value;
                OnPropertyChanged();
            }
        }
        public string PhoneNumbers {
            get => phoneNumbers;
            set {
                phoneNumbers = value;
                OnPropertyChanged();
            }
        }
        public string Customer {
            get => customer;
            set {
                customer = value;
                OnPropertyChanged();
            }
        }
        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
