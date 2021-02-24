using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    class HomeViewModel : BaseViewModel
    {
        private ObservableCollection<CheckAndHeightPair> filterAreaSections = new ObservableCollection<CheckAndHeightPair>() {
            new CheckAndHeightPair(true, 143),
            new CheckAndHeightPair(true, 180),
            new CheckAndHeightPair(true, 153),
            new CheckAndHeightPair(false, 50),
            new CheckAndHeightPair(false, 50),
            new CheckAndHeightPair(false, 50),
            new CheckAndHeightPair(false, 50),
            new CheckAndHeightPair(false, 50),
            new CheckAndHeightPair(true, 200)
        };
        private ObservableCollection<BaseRealtorObject> currentObjectList = new ObservableCollection<BaseRealtorObject>() { new RandomFlatGenerator.FlatGenerator().CreateFlat()};
        private List<ObservableCollection<BaseRealtorObject>> objectLists = new List<ObservableCollection<BaseRealtorObject>>();
        private List<BaseRealtorObject> filteredObjects = new List<BaseRealtorObject>();
        private List<BaseRealtorObject> allObjects = new List<BaseRealtorObject>();
        private CustomCommand openOrCloseFilterSection;
        private CustomCommand testCommand;
        private CustomCommand filterCollection;
        private bool loadingScreenVisibility = false;

        private bool isFlat = false;
        private bool isHouse = false;
        private bool isPlot = false;
        private Int32 minimumPrice = 0;
        private Int32 maximumPrice = 20000000;
        private bool hasMortgage = false;
        private Int32 minimumArea = 0;
        private Int32 maximumArea = 1500;

        public bool LoadingScreenVisibility {
            get => loadingScreenVisibility;
            set {
                loadingScreenVisibility = value;
                OnPropertyChanged();
            }
        }
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj => {
            var flatGenerator = new RandomFlatGenerator.FlatGenerator();
            foreach (BaseRealtorObject bro in flatGenerator.CreateFlatList(AllObjects.Count, AllObjects.Count + 50)) {
                AllObjects.Add(bro);
            }
            /*
            for (int i = 0; i < 10; i++) {
                var flat = flatGenerator.CreateFlat();
                flat.Info.Description = "Гарри Поттер и Лорем ипсум долор сит амет, справа находится описание, слева - дорога на Эльдорадо";
                CurrentObjectList.Add(flat);
            }*/
        }));
        
        public CustomCommand FilterCollection => filterCollection ?? (filterCollection = new CustomCommand(obj => {
            
            FilteredObjects.Clear();
            FilteredObjects.AddRange(AllObjects.Where(x => MaximumPrice >= x.Cost.Price).Where(x => MinimumPrice <= x.Cost.Price).ToList());
            FilteredObjects = FilteredObjects.Where(x => MaximumArea >= x.Cost.Area).Where(x => MinimumArea <= x.Cost.Area).ToList();
            if (HasMortgage) {
                FilteredObjects.RemoveAll(x => x.Cost.HasMortgage == false);
            }
            FilterByObjectType();
            CurrentObjectList = new ObservableCollection<BaseRealtorObject>(FilteredObjects);
            
        }));
        private void FilterByObjectType() {
            if (!IsPlot) {
                FilteredObjects.RemoveAll(x => x.ObjectType == "Земельный участок");
            }
            if (!IsHouse) {
                FilteredObjects.RemoveAll(x => x.ObjectType == "Частный дом");
            }
            if (!IsFlat) {
                FilteredObjects.RemoveAll(x => x.ObjectType == "Квартира");
            }
        }
        public List<BaseRealtorObject> FilteredObjects {
            get => filteredObjects;
            set {
                filteredObjects = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<BaseRealtorObject> CurrentObjectList {
            get => currentObjectList;
            set {
                currentObjectList = value;
                OnPropertyChanged();
            }
        }
        public List<ObservableCollection<BaseRealtorObject>> ObjectLists {
            get => objectLists;
            set => objectLists = value;
        }
        public List<BaseRealtorObject> AllObjects {
            get => allObjects;
            set => allObjects = value;
        }
        public CustomCommand OpenOrCloseFilterSection => openOrCloseFilterSection ?? (openOrCloseFilterSection = new CustomCommand(obj => {
            object[] objects = obj as object[];
            byte index = Convert.ToByte(objects[0]);
            Int16 height = Convert.ToInt16(objects[1]);

            if (!FilterAreaSections[index].Check) {
                FilterAreaSections[index].Height = 50;
            } else {
                FilterAreaSections[index].Height = height;
            }
        }));
        public ObservableCollection<CheckAndHeightPair> FilterAreaSections => filterAreaSections;

        public int MinimumPrice {
            get => minimumPrice;
            set {
                if (value > maximumPrice) {
                    MaximumPrice = value;
                }
                minimumPrice = value;
                OnPropertyChanged();
            }
        }
        public int MaximumPrice {
            get => maximumPrice;
            set {
                if (value < minimumPrice) {
                    MinimumPrice = value;
                }
                maximumPrice = value;
                OnPropertyChanged();
            }
        }
        public bool HasMortgage {
            get => hasMortgage;
            set => hasMortgage = value;
        }
        public int MinimumArea {
            get => minimumArea;
            set {
                if (value > maximumArea) {
                    MaximumArea = value;
                }
                minimumArea = value;
                OnPropertyChanged();
            }
        }
        public int MaximumArea {
            get => maximumArea;
            set {
                if (value < minimumArea) {
                    MinimumArea = value;
                }
                maximumArea = value;
                OnPropertyChanged();
            }
        }
        public bool IsFlat {
            get => isFlat;
            set {
                isFlat = value;

                OnPropertyChanged();
            }
        }
        public bool IsHouse {
            get => isHouse;
            set {
                isHouse = value;
                OnPropertyChanged();
            }
        }
        public bool IsPlot {
            get => isPlot;
            set {
                isPlot = value;
                OnPropertyChanged();
            }
        }
    }
}
