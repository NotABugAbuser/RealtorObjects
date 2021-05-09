using RandomFlatGenerator;
using RealtorObjects.Model;
using RealtyModel.Events.Realty;
using RealtyModel.Events.UI;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        #region Fields
        private int currentPage = 1;
        private double widthOfFilters = 200;
        private Filter filter = new Filter();
        private CustomCommand delete;
        private CustomCommand modify;
        private CustomCommand goToPage;
        private CustomCommand testCommand;
        private CustomCommand sendQuery;
        private CustomCommand openCloseFilters;
        private CustomCommand createRealtorObject;
        private CustomCommand openOrCloseFilterSection;
        private ObservableCollection<int> pages = new ObservableCollection<int>();
        private List<BaseRealtorObject> allObjects = new List<BaseRealtorObject>();
        private ObservableCollection<BaseRealtorObject> currentObjectList = new ObservableCollection<BaseRealtorObject>() { };
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
        private List<ObservableCollection<BaseRealtorObject>> objectLists = new List<ObservableCollection<BaseRealtorObject>>();
        public event FlatButtonPressedEventHandler FlatButtonPressed;
        public event DeleteButtonPressedEventHandler DeleteButtonPressed;
        public event QueryCreatedEventHandler QueryCreated;
        #endregion
        #region Properties
        public int CurrentPage
        {
            get => currentPage;
            set
            {
                currentPage = value;
                OnPropertyChanged();
            }
        }
        public double WidthOfFilters
        {
            get => widthOfFilters;
            set
            {
                widthOfFilters = value;
                OnPropertyChanged();
            }
        }
        public Filter Filter
        {
            get => filter;
            set => filter = value;
        }
        public List<BaseRealtorObject> AllObjects
        {
            get => allObjects;
            set => allObjects = value;
        }
        public List<ObservableCollection<BaseRealtorObject>> ObjectLists
        {
            get => objectLists;
            set => objectLists = value;
        }
        public ObservableCollection<int> Pages
        {
            get => pages; set => pages = value;
        }
        public ObservableCollection<BaseRealtorObject> CurrentObjectList
        {
            get => currentObjectList;
            set
            {
                currentObjectList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<CheckAndHeightPair> FilterAreaSections => filterAreaSections;
        public LocationOptions LocationOptions { get; set; }
        #endregion

        public CustomCommand OpenCloseFilters => openCloseFilters ?? (openCloseFilters = new CustomCommand(obj =>
        {
            if (WidthOfFilters == 200)
            {
                WidthOfFilters = 0;
            }
            else
            {
                WidthOfFilters = 200;
            }
        }));
        public CustomCommand CreateRealtorObject => createRealtorObject ?? (createRealtorObject = new CustomCommand(obj =>
        {
            string type = (string)obj;
            if (type == "Flat")
                FlatButtonPressed?.Invoke(this, new FlatButtonPressedEventArgs(true, new Flat(), LocationOptions));
        }));
        public CustomCommand Modify => modify ?? (modify = new CustomCommand(obj =>
        {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            if (CheckAccess(bro.Agent, ((App)Application.Current).Credential.Name))
                if (bro is Flat flat)
                    FlatButtonPressed?.Invoke(this, new FlatButtonPressedEventArgs(false, flat));
        }));
        public CustomCommand Delete => delete ?? (delete = new CustomCommand(obj =>
        {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            if (CheckAccess(bro.Agent, ((App)Application.Current).Credential.Name))
                DeleteButtonPressed?.Invoke(this, new DeleteButtonPressedEventArgs(bro));
        }));
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj => { }));
        public CustomCommand SendQuery => sendQuery ?? (sendQuery = new CustomCommand(obj => QueryCreated?.Invoke(this, new QueryCreatedEventArgs(Filter))));
        public CustomCommand OpenOrCloseFilterSection => openOrCloseFilterSection ?? (openOrCloseFilterSection = new CustomCommand(obj =>
        {
            object[] objects = obj as object[];
            byte index = Convert.ToByte(objects[0]);
            Int16 height = Convert.ToInt16(objects[1]);

            if (!FilterAreaSections[index].Check)
            {
                FilterAreaSections[index].Height = 50;
            }
            else
            {
                FilterAreaSections[index].Height = height;
            }
        }));
        public CustomCommand GoToPage => goToPage ?? (goToPage = new CustomCommand(obj =>
        {
            Int16 index = (Int16)(Convert.ToInt16(obj) - 1);
            Pages.Clear();
            CalculatePages(index);
            CurrentPage = index + 1;
            CurrentObjectList = ObjectLists[index];
        }));

        public HomeViewModel()
        {
        }

        private void SplitFilteredCollection(List<BaseRealtorObject> filteredObjects, byte count)
        {
            //разбиение фильтрованной коллекции на parts частей по count объектов
            Int16 parts = (Int16)Math.Ceiling(filteredObjects.Count / (double)count);
            List<List<BaseRealtorObject>> lists = Enumerable.Range(0, parts).AsParallel().Select(x => filteredObjects.Skip(x * count).Take(count).ToList()).ToList();
            ObjectLists.Clear();
            CurrentObjectList.Clear();
            foreach (List<BaseRealtorObject> ol in lists)
            {
                ObjectLists.Add(new ObservableCollection<BaseRealtorObject>(ol));
            }
            if (ObjectLists.Count != 0)
            {
                CurrentObjectList = ObjectLists[0];
            }
        }
        private void CalculatePages(short currentPage)
        {
            int count = ObjectLists.Count;
            if (count < 15)
            {
                for (int i = 0; i < count; i++) { Pages.Add(i + 1); }
            }
            else
            {
                int left = 7;
                int right = 7;
                if (currentPage + 8 > count)
                {
                    int difference = -(count - 8 - currentPage);
                    left += difference;
                    right -= difference;
                }
                if (currentPage - 7 < 0)
                {
                    int difference = -(currentPage - 7);
                    left -= difference;
                    right += difference;
                }
                for (int i = left; i > 0; i--) { Pages.Add(currentPage - i + 1); }
                for (int i = 0; i < right + 1; i++) { Pages.Add(currentPage + i + 1); }
            }
        }
        private bool CheckAccess(string objectAgent, string currentAgent)
        {
            if (objectAgent == currentAgent)
                return true;
            else
            {
                MessageBox.Show("У вас нет права на доступ к этому объекту");
                return false;
            }
        }
    }
}
