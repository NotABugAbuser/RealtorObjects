using RandomFlatGenerator;
using RealtorObjects.Model;
using RealtorObjects.View;
using RealtyModel.Events.Realty;
using RealtyModel.Events.UI;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Model.RealtyObjects;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace RealtorObjects.ViewModel
{
    public class HomeViewModel : BaseViewModel {
        #region Fields
        private string currentAgentName = "";
        private int currentPage = 1;
        private double widthOfFilters = 200;
        private Filter filter = new Filter();
        private LocationOptions locationOptions = new LocationOptions();
        private CustomCommand modify;
        private CustomCommand goToPage;
        private CustomCommand openCloseFilters;
        private CustomCommand sendQuery;
        private ObservableCollection<int> pages = new ObservableCollection<int>();
        private ObservableCollection<BaseRealtorObject> currentObjectList = new ObservableCollection<BaseRealtorObject>() { };
        private List<ObservableCollection<BaseRealtorObject>> objectLists = new List<ObservableCollection<BaseRealtorObject>>();
        private CustomCommand createFlat;
        #endregion
        #region Properties
        public int CurrentPage {
            get => currentPage;
            set {
                currentPage = value;
                OnPropertyChanged();
            }
        }
        public double WidthOfFilters {
            get => widthOfFilters;
            set {
                widthOfFilters = value;
                OnPropertyChanged();
            }
        }
        public Filter Filter {
            get => filter;
            set => filter = value;
        }
        public List<ObservableCollection<BaseRealtorObject>> ObjectLists {
            get => objectLists;
            set => objectLists = value;
        }
        public ObservableCollection<int> Pages {
            get => pages; set => pages = value;
        }
        public ObservableCollection<BaseRealtorObject> CurrentObjectList {
            get => currentObjectList;
            set {
                currentObjectList = value;
                OnPropertyChanged();
            }
        }

        #endregion
        public HomeViewModel() {
            this.LocationOptions = Client.RequestLocationOptions();
        }
        public CustomCommand SendQuery => sendQuery ?? (sendQuery = new CustomCommand(obj => {
            CurrentObjectList.Clear();
            CurrentObjectList = Client.RequestRealtorObjects(Filter);
        }));
        public CustomCommand CreateFlat => createFlat ?? (createFlat = new CustomCommand(obj => {
            new FlatFormV2(new FlatFormViewModel(true, CurrentAgentName)).Show();
        }));
        public CustomCommand OpenCloseFilters => openCloseFilters ?? (openCloseFilters = new CustomCommand(obj => {
            if (WidthOfFilters == 200) {
                WidthOfFilters = 0;
            } else {
                WidthOfFilters = 200;
            }
        }));
        public CustomCommand Modify => modify ?? (modify = new CustomCommand(obj => {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            if (bro is Flat) {
                new FlatFormV2(new FlatFormViewModel(false, CurrentAgentName)).Show();
            }
        }));
        public CustomCommand GoToPage => goToPage ?? (goToPage = new CustomCommand(obj => {
            Int16 index = (Int16)(Convert.ToInt16(obj) - 1);
            Pages.Clear();
            CalculatePages(index);
            CurrentPage = index + 1;
            CurrentObjectList = ObjectLists[index];
        }));

        public string CurrentAgentName {
            get => currentAgentName; set => currentAgentName = value;
        }
        public LocationOptions LocationOptions {
            get => locationOptions; 
            set 
            {
                locationOptions = value;
                OnPropertyChanged();
            }
        }
        private void CalculatePages(short currentPage) {
            int count = ObjectLists.Count;
            if (count < 15) {
                for (int i = 0; i < count; i++) { Pages.Add(i + 1); }
            } else {
                int left = 7;
                int right = 7;
                if (currentPage + 8 > count) {
                    int difference = -(count - 8 - currentPage);
                    left += difference;
                    right -= difference;
                }
                if (currentPage - 7 < 0) {
                    int difference = -(currentPage - 7);
                    left -= difference;
                    right += difference;
                }
                for (int i = left; i > 0; i--) { Pages.Add(currentPage - i + 1); }
                for (int i = 0; i < right + 1; i++) { Pages.Add(currentPage + i + 1); }
            }
        }
        private bool CheckAccess(string objectAgent, string currentAgent) {
            if (objectAgent == currentAgent)
                return true;
            else {
                MessageBox.Show("У вас нет права на доступ к этому объекту");
                return false;
            }
        }
    }
}
