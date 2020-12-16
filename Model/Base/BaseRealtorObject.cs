using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    public class BaseRealtorObject : INotifyPropertyChanged
    {
        PersonInfo personInfo = new PersonInfo();
        Location location = new Location();
        Cost cost = new Cost();
        public PersonInfo PersonInfo {
            get => personInfo;
            set {
                personInfo = value;
                OnPropertyChanged();
            }
        }
        public Location Location {
            get => location;
            set {
                location = value;
                OnPropertyChanged();
            }
        }
        public Cost Cost {
            get => cost;
            set {
                cost = value;
                OnPropertyChanged();
            }
        }
        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
