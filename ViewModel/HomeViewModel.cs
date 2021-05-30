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
using System.Windows.Threading;

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
        private CustomCommand createRealtorObject;

        private ObservableCollection<int> pages = new ObservableCollection<int>();
        private ObservableCollection<BaseRealtorObject> currentObjectList = new ObservableCollection<BaseRealtorObject>() { };
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
        public LocationOptions LocationOptions { get; set; }
        public Dispatcher Dispatcher { get; set; }
        #endregion

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
        public CustomCommand SendQuery => sendQuery ?? (sendQuery = new CustomCommand(obj => RequestQuery()));
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

        public void OnNewQueryResultReceived(QueryResultReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ObjectLists.Add(e.QueryObjects);
                GoToPage.Execute(1);
            });
        }
        public void OnQueryResultReceived(QueryResultReceivedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ObjectLists.Add(e.QueryObjects);
            });
        }
        private void RequestQuery()
        {
            Dispatcher.Invoke(() =>
            {
                Pages.Clear();
                ObjectLists.Clear();
                CurrentObjectList.Clear();
            });
            QueryCreated?.Invoke(this, new QueryCreatedEventArgs(Filter));
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
