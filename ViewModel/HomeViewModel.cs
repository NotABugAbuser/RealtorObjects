using RandomFlatGenerator;
using RealtorObjects.Model;
using RealtorObjects.View;
using RealtyModel.Event;
using RealtyModel.Event.RealtyEvents;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Model.RealtyObjects;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
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
        private CustomCommand filterCollection;
        private CustomCommand openCloseFilters;
        private CustomCommand createRealtorObject;
        private CustomCommand openOrCloseFilterSection;
        private ObservableCollection<int> pages = new ObservableCollection<int>() {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            123
        };
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
        public event OpeningFlatFormEventHandler OpeningFlatForm;
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
                OpeningFlatForm?.Invoke(this, new OpeningFlatFormEventArgs(true, new Flat(), LocationOptions));

            //if (type == "House") ;
        }));
        public CustomCommand Modify => modify ?? (modify = new CustomCommand(obj =>
        {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            if (CheckAccess(bro.Agent, ((App)Application.Current).Credential.Name))
            {
                if (bro is Flat flat)
                    OpeningFlatForm?.Invoke(this, new OpeningFlatFormEventArgs(false, flat));
                //    if (bro is House house) RealtorObjectOperator.ModifyHouse(house);
            }
        }));
        public CustomCommand Delete => delete ?? (delete = new CustomCommand(obj =>
        {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            if (CheckAccess(bro.Agent, ((App)Application.Current).Credential.Name))
                ((App)Application.Current).OperationManagement.SendRealtyData(bro.Id, OperationType.Remove, bro.Type);
        }));
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj => { }));
        public CustomCommand FilterCollection => filterCollection ?? (filterCollection = new CustomCommand(obj =>
        {
            List<BaseRealtorObject> filteredObjects = Filter.CreateFilteredList(AllObjects);
            Debug.WriteLine($"Фильтрованная: {filteredObjects.Count}");
            SplitFilteredCollection(filteredObjects, 25);
            Pages.Clear();
            CalculatePages(1);
            CurrentPage = 1;
            Debug.WriteLine($"Фильтрованная {filteredObjects.Count} и листов {ObjectLists.Count}");
            GC.Collect();
        }));
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
            using (var context = new DataBaseContext())
            {
                ClearDB(context);
                AllObjects.AddRange(context.Flats);
                AllObjects.AddRange(context.Houses);
            }
        }

        internal void ClearDB(DataBaseContext context)
        {
            context.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Customers'");
            context.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Albums'");
            context.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Cities'");
            context.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Districts'");
            context.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Streets'");
            context.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Flats'");
            context.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Houses'");
            context.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Locations'");
            context.Flats.Local.Clear();
            context.Houses.Local.Clear();
            context.Locations.Local.Clear();
            context.Cities.Local.Clear();
            context.Districts.Local.Clear();
            context.Streets.Local.Clear();
            context.Albums.Local.Clear();
            context.Customers.Local.Clear();
            context.SaveChanges();
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

        private void TestMethod()
        {
            FlatGenerator flatGenerator = new FlatGenerator();
            Flat flat = flatGenerator.CreateFlat();
            flat.Id = 9999;
            flat.Agent = "ГвоздиковЕА";
            flat.Status = Status.Planned;
            CurrentObjectList.Add(flat);
            AllObjects.Add(flat);
        }
    }
}
