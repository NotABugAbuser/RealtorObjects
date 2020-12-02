using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace RealtorObjects.Model
{
    class Location : INotifyPropertyChanged
    {
        String city = "";
        String district = "";
        String street = "";
        Int16 houseNumber = 0;
        Int16 flatNumber = 0;
        bool banner = false;
        bool exchange = false;
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

        public bool Exchange {
            get => exchange;
            set {
                exchange = value;
                OnPropertyChanged();
            }
        }
        public bool Banner {
            get => banner;
            set {
                banner = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
