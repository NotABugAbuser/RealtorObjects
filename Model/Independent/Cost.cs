using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    public class Cost : INotifyPropertyChanged
    {
        double area = 0;
        double multiplier = 1000;
        double pseudoPrice = 0;
        double realPrice = 0;
        bool hasPercents = true;
        double percents = 0;
        bool hasVAT = true; // НДС
        bool hasMortgage = false; //Ипотека

        public double Area {
            get => area;
            set {
                if (value >= 0 && value <= 1000)
                    area = value;
                OnPropertyChanged();
            }
        }
        public double Multiplier {
            get => multiplier;
            set {
                if (value >= 0 && value <= 1000000)
                    multiplier = value;
                    RealPrice = PseudoPrice * Multiplier;
                OnPropertyChanged();
            }
        }
        public double PseudoPrice {
            get => pseudoPrice;
            set {
                if (value >= 0 && value < 1000) {
                    pseudoPrice = value;
                    RealPrice = PseudoPrice * Multiplier;
                }
                OnPropertyChanged();
            }
        }
        public double RealPrice {
            get => realPrice;
            private set {
                if (value >= 0 && value <= 1000000000)
                    realPrice = value;
                OnPropertyChanged();
            }
        }
        public bool HasVAT {
            get => hasVAT;
            set {
                hasVAT = value;
                OnPropertyChanged();
            }
        }
        public bool HasMortgage {
            get => hasMortgage;
            set {
                hasMortgage = value;
                OnPropertyChanged();
            }
        }

        public bool HasPercents {
            get => hasPercents;
            set {
                hasPercents = value;
                OnPropertyChanged();
            }
        }
        public double Percents {
            get => percents;
            set {
                if (value >= 0 && value <= 100)
                    percents = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
