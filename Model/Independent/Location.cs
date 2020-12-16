using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace RealtorObjects.Model
{
    public class Location : INotifyPropertyChanged
    {
        String city = "";
        String district = "";
        String street = "";
        Int16 houseNumber = 0;
        Int16 flatNumber = 0;
        bool hasBanner = false;
        bool hasEchange = false;
        public string City {
            get => city;
            set {
                city = value;
                OnPropertyChanged();
            }
        }
        public string District {
            get => district;
            set {
                district = value;
                OnPropertyChanged();
            }
        }
        public string Street {
            get => street;
            set {
                street = value;
                OnPropertyChanged();
            }
        }
        public Int16 HouseNumber {
            get => houseNumber;
            set {
                houseNumber = value;
                OnPropertyChanged();
            }
        }
        public Int16 FlatNumber {
            get => flatNumber;
            set {
                flatNumber = value;
                OnPropertyChanged();
            }
        }

        public bool HasExchange {
            get => hasEchange;
            set {
                hasEchange = value;
                OnPropertyChanged();
            }
        }
        public bool HasBanner {
            get => hasBanner;
            set {
                hasBanner = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
