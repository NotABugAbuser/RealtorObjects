using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace RealtorObjects.Model
{
    public class BaseInfo : INotifyPropertyChanged
    {
        int year = 2000;
        int roomCount = 0;
        double general = 0;
        double living = 0;
        double kitchen = 0;
        string description = "";
        string water = "";
        Double ceiling = 2;
        string convenience = "";
        string heating = "";
        string condition = "";
        public string Water {
            get => water;
            set {
                water = value;
                OnPropertyChanged();
            }
        }
        public Double Ceiling {
            get => ceiling;
            set {
                ceiling = value;
                OnPropertyChanged();
            }
        }
        public string Convenience {
            get => convenience;
            set {
                convenience = value;
                OnPropertyChanged();
            }
        }
        public string Heating {
            get => heating;
            set {
                heating = value;
                OnPropertyChanged();
            }
        }

        public string Condition {
            get => condition;
            set {
                condition = value;
                OnPropertyChanged();
            }
        }

        public int Year {
            get => year;
            set {
                if (value >= 1900 && value <= 2100)
                    year = value;
                OnPropertyChanged();
            }
        }
        public int RoomCount {
            get => roomCount;
            set {
                if (value >= 0 && value <= 100)
                    roomCount = value;
                OnPropertyChanged();
            }
        }
        public double General {
            get => general;
            set {
                if (value >= 0 && value <= 100)
                    general = value;
                OnPropertyChanged();
            }
        }
        public double Living {
            get => living;
            set {
                if (value >= 0 && value <= 100)
                    living = value;
                OnPropertyChanged();
            }
        }
        public double Kitchen {
            get => kitchen;
            set {
                if (value >= 0 && value <= 100)
                    kitchen = value;
                OnPropertyChanged();
            }
        }
        public string Description {
            get => description;
            set {
                description = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
