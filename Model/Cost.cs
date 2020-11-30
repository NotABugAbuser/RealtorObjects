using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    class Cost : INotifyPropertyChanged
    {
        double area = 0;
        double multiplier = 0;
        double pseudoPrice = 0;
        double realprice = 0;
        bool vAT = false; // НДС
        bool mortgage = false; //Ипотека

        public double Area {
            get => area;
            set {
                area = value;
                OnPropertyChanged();
            }
        }
        public double Multiplier {
            get => multiplier;
            set {
                multiplier = value;
                OnPropertyChanged();
            }
        }
        public double PseudoPrice {
            get => pseudoPrice;
            set {
                pseudoPrice = value;
                OnPropertyChanged();
            }
        }
        public double Realprice {
            get => realprice;
            set {
                realprice = value;
                OnPropertyChanged();
            }
        }
        public bool VAT {
            get => vAT;
            set {
                vAT = value;
                OnPropertyChanged();
            }
        }
        public bool Mortgage {
            get => mortgage;
            set {
                mortgage = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
