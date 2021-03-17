using RandomFlatGenerator;
using RealtyModel.Model;
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
            new CheckAndHeightPair(true, 143),
            new CheckAndHeightPair(true, 180),
            new CheckAndHeightPair(true, 153),
            new CheckAndHeightPair(false, 50),
            new CheckAndHeightPair(false, 50),
            new CheckAndHeightPair(false, 50),
            new CheckAndHeightPair(false, 50),
            new CheckAndHeightPair(false, 50),
            new CheckAndHeightPair(false, 50),
        };
        private ObservableCollection<BaseRealtorObject> currentObjectList = new ObservableCollection<BaseRealtorObject>() { };
        private List<ObservableCollection<BaseRealtorObject>> objectLists = new List<ObservableCollection<BaseRealtorObject>>();
        private List<BaseRealtorObject> allObjects = new List<BaseRealtorObject>();
        private CustomCommand openOrCloseFilterSection;
        private CustomCommand testCommand;
        private CustomCommand filterCollection;
        private bool loadingScreenVisibility = false;
        private Filter filter = new Filter();
        public HomeViewModel() {
            FlatGenerator flatGenerator = new FlatGenerator();
            Flat flat = flatGenerator.CreateFlat();
            flat.Status = Status.Archived;
            CurrentObjectList.Add(flat);
        }
        public bool LoadingScreenVisibility {
            get => loadingScreenVisibility;
            set {
                loadingScreenVisibility = value;
                OnPropertyChanged();
            }
        }
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj => {
            FlatGenerator flatGenerator = new FlatGenerator();
            foreach (BaseRealtorObject bro in flatGenerator.CreateFlatList(AllObjects.Count, AllObjects.Count + 50)) {
                AllObjects.Add(bro);
            }
        }));

        public CustomCommand FilterCollection => filterCollection ?? (filterCollection = new CustomCommand(obj => {
            CurrentObjectList.Clear();
            DateTime dateTime = new DateTime(2021, 4, 16);
            DateTime dateTime2 = new DateTime(2020, 4, 16);
            CurrentObjectList = new ObservableCollection<BaseRealtorObject>(Filter.CreateFilteredList(AllObjects));
            GC.Collect();
        }));
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
        public Filter Filter {
            get => filter; 
            set => filter = value;
        }
    }
}
