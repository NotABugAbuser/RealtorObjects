using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    public class FlatInfo : BaseInfo
    {
        string material = "";
        string fund = "";
        string type = "";
        string windows = "";
        int kvl = 0;
        bool isPrivatised = false;
        bool isSeparated = true;
        bool hasImprovedLayout = false;
        bool hasRenovation = false;
        bool hasChute = false;
        string typeOfRooms = "Раздельные";
        string rooms = "";
        bool hasGarage = false;
        bool hasElevator = false;
        bool isCorner = false;
        string balcony = "";
        string loggia = "";
        string bath = "";
        string bathroom = "";
        string floor = "";

        public string Balcony {
            get => balcony;
            set {
                balcony = value;
                OnPropertyChanged();
            }
        }
        public string Loggia {
            get => loggia;
            set {
                loggia = value;
                OnPropertyChanged();
            }
        }
        public string Bath {
            get => bath;
            set {
                bath = value;
                OnPropertyChanged();
            }
        }
        public string Bathroom {
            get => bathroom;
            set {
                bathroom = value;
                OnPropertyChanged();
            }
        }
        public string Floor {
            get => floor;
            set {
                floor = value;
                OnPropertyChanged();
            }
        }

        public string Material {
            get => material;
            set {
                material = value;
                OnPropertyChanged();
            }
        }
        public string Fund {
            get => fund;
            set {
                fund = value;
                OnPropertyChanged();
            }
        }
        public string Type {
            get => type;
            set {
                type = value;
                OnPropertyChanged();
            }
        }
        public string Windows {
            get => windows;
            set {
                windows = value;
                OnPropertyChanged();
            }
        }
        public int Kvl {
            get => kvl;
            set {
                if (value >= 0)
                    kvl = value;
                OnPropertyChanged();
            }
        }
        public bool IsPrivatised {
            get => isPrivatised;
            set {
                isPrivatised = value;
                OnPropertyChanged();
            }
        }
        public bool HasImprovedLayout {
            get => hasImprovedLayout;
            set {
                hasImprovedLayout = value;
                OnPropertyChanged();
            }
        }
        public bool HasRenovation {
            get => hasRenovation;
            set {
                hasRenovation = value;
                OnPropertyChanged();
            }
        }
        public bool HasChute {
            get => hasChute;
            set {
                hasChute = value;
                OnPropertyChanged();
            }
        }
        public string TypeOfRooms {
            get => typeOfRooms;
        }
        public string Rooms {
            get => rooms;
            set {
                rooms = value;
                OnPropertyChanged();
            }
        }
        public bool HasGarage {
            get => hasGarage;
            set {
                hasGarage = value;
                OnPropertyChanged();
            }
        }
        public bool HasElevator {
            get => hasElevator;
            set {
                hasElevator = value;
                OnPropertyChanged();
            }
        }
        public bool IsCorner {
            get => isCorner;
            set {
                isCorner = value;
                OnPropertyChanged();
            }
        }
        public bool IsSeparated {
            get => isSeparated;
            set {
                isSeparated = value;
                if (isSeparated)
                    typeOfRooms = "Раздельные";
                else
                    typeOfRooms = "Смежные";
                OnPropertyChanged();
            }
        }
    }
}
