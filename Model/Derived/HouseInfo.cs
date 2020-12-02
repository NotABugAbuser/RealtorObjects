using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    class HouseInfo : BaseInfo
    {
        string typeOfHouse = "";
        int levels = 0;
        int floors = 0;
        bool hasDemarcation = false; //межевание
        string earthCategory = "";
        bool hasSlope = false;
        float hundreds = 0f;
        float hectar = 0f;
        bool hasElectricity = false;
        string walls = "";
        string yard = "";
        string roof = "";
        string sewerage = "";
        string gas = ""; 
        

        public string Walls {
            get => walls;
            set {
                walls = value;
                OnPropertyChanged();
            }
        }
        public string Yard {
            get => yard;
            set {
                yard = value;
                OnPropertyChanged();
            }
        }
        public string Roof {
            get => roof;
            set {
                roof = value;
                OnPropertyChanged();
            }
        }
        public string Sewerage {
            get => sewerage;
            set {
                sewerage = value;
                OnPropertyChanged();
            }
        }
        public string Gas {
            get => gas;
            set {
                gas = value;
                OnPropertyChanged();
            }
        }

        public string TypeOfHouse {
            get => typeOfHouse;
            set {
                typeOfHouse = value;
                OnPropertyChanged();
            }
        }
        public int Levels {
            get => levels;
            set {
                levels = value;
                OnPropertyChanged();
            }
        }
        public int Floors {
            get => floors;
            set {
                floors = value;
                OnPropertyChanged();
            }
        }
        public bool HasDemarcation {
            get => hasDemarcation;
            set {
                hasDemarcation = value;
                OnPropertyChanged();
            }
        }
        public string EarthCategory {
            get => earthCategory;
            set {
                earthCategory = value;
                OnPropertyChanged();
            }
        }
        public bool HasSlope {
            get => hasSlope;
            set {
                hasSlope = value;
                OnPropertyChanged();
            }
        }
        public float Hundreds {
            get => hundreds;
            set {
                hundreds = value;
                OnPropertyChanged();
            }
        }
        public float Hectar {
            get => hectar;
            set {
                hectar = value;
                OnPropertyChanged();
            }
        }
        public bool HasElectricity {
            get => hasElectricity;
            set {
                hasElectricity = value;
                OnPropertyChanged();
            }
        }
    }
}
