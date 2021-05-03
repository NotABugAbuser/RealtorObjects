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
        public event UpdateFinishedEventHandler UpdateFinished;
        public event OpeningFlatFormEventHandler OpeningFlatForm;
        private DataBaseContext dataBase = new DataBaseContext();
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
                OpeningFlatForm?.Invoke(this, new OpeningFlatFormEventArgs(true, new Flat(), GetLocationOptions()));

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
            {
                AllObjects.RemoveAll(x => x.Id == bro.Id);
                CurrentObjectList.Remove(bro);
                Operation operation = new Operation(((App)Application.Current).Credential.Name, bro.Id.ToString(), OperationDirection.Realty, OperationType.Remove);
            }
        }));
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj =>
        {
            Flat dbFlat = dataBase.Flats.Local.Last<Flat>();
            dbFlat.Location.FlatNumber++;
            dbFlat.Agent = "Dima";
            ((App)Application.Current).Client.OutcomingOperations.Enqueue(new Operation("none", GetLastUpdateTime(), OperationDirection.Realty, OperationType.Add, TargetType.Flat)
            {
                Data = JsonSerializer.Serialize(dbFlat)
            });
            dbFlat.Location.FlatNumber--;
        }));
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
            ClearDB();
            AllObjects.AddRange(dataBase.Flats);
            AllObjects.AddRange(dataBase.Houses);
        }

        internal void ClearDB()
        {
            dataBase.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Customers'");
            dataBase.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Albums'");
            dataBase.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Cities'");
            dataBase.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Districts'");
            dataBase.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Streets'");
            dataBase.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Flats'");
            dataBase.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Houses'");
            dataBase.Database.ExecuteSqlCommand("update sqlite_sequence set seq = 0 where name = 'Locations'");
            dataBase.Flats.Local.Clear();
            dataBase.Houses.Local.Clear();
            dataBase.Locations.Local.Clear();
            dataBase.Cities.Local.Clear();
            dataBase.Districts.Local.Clear();
            dataBase.Streets.Local.Clear();
            dataBase.Albums.Local.Clear();
            dataBase.Customers.Local.Clear();
            dataBase.SaveChanges();
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
        private void FilterCollectionMeth()
        {
            ((App)Application.Current).Dispatcher.Invoke(() =>
            {
                FilterCollection.Execute(new object());
            });
        }

        internal void GetUpdate()
        {
            ((App)Application.Current).Client.OutcomingOperations.Enqueue(new Operation("none", GetLastUpdateTime(), OperationDirection.Realty, OperationType.Update));
        }
        //Проверить сохраняет ли в бд под теми же Id или задаёт их по новой
        internal void ReceiveDbUpdate(ReceivedDbUpdateEventArgs e)
        {
            try
            {
                if (e.TargetType == TargetType.All)
                {
                    String[] objects = JsonSerializer.Deserialize<String[]>((String)e.UpdateData);
                    if (!String.IsNullOrWhiteSpace(objects[0]))
                    {
                        Flat[] flats = JsonSerializer.Deserialize<Flat[]>(objects[0]);
                        AllObjects.AddRange(flats);
                        dataBase.Flats.AddRange(flats);
                    }
                    if (!String.IsNullOrWhiteSpace(objects[1]))
                    {
                        House[] houses = JsonSerializer.Deserialize<House[]>(objects[1]);
                        AllObjects.AddRange(houses);
                        dataBase.Houses.AddRange(houses);
                    }
                }
                else if (e.TargetType == TargetType.Album)
                {
                    Photo[] photos = JsonSerializer.Deserialize<Photo[]>((String)e.UpdateData);
                    dataBase.Photos.AddRange(photos);
                    foreach (BaseRealtorObject bro in AllObjects)
                        bro.Album.GetPhotosFromDB(dataBase.Photos.Local);
                }
                else if (e.TargetType == TargetType.None)
                {
                    Debug.WriteLine("Start of update");
                    dataBase.SaveChanges();
                    FilterCollectionMeth();
                    WriteLastUpdateTime();
                    Debug.WriteLine("End of update");
                    UpdateFinished?.Invoke(this, new UpdateFinishedEventArgs());
                }
                //if (GetLastUpdateTime() == "never")
                //{
                //}
                //else
                //{
                //    if (objects[0] != null)
                //    {
                //        Flat[] flats = JsonSerializer.Deserialize<Flat[]>(objects[0]);
                //        foreach (Flat flat in flats)
                //        {
                //            Flat dbFlat = dataBase.Flats.Find(flat.Id);
                //            if (dbFlat == null)
                //            {
                //                dataBase.Flats.Add(flat);
                //                AllObjects.Add(flat);
                //            }
                //            else
                //            {
                //                dbFlat = flat;
                //                Flat listFlat = (Flat)AllObjects.FirstOrDefault(fl => fl.Id == flat.Id);
                //                listFlat = flat;
                //            }
                //        }
                //        AllObjects.AddRange(flats);
                //        dataBase.Flats.AddRange(flats);
                //    }
                //    if (objects[1] != null)
                //    {
                //    }
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{DateTime.Now} ReceiveUpdate {ex.Message}");
                //Запросить обновление снова
            }
        }
        internal void AddFlat(Flat flat)
        {
            dataBase.Flats.Local.Add(flat);
            dataBase.SaveChanges();
            AllObjects.Add(flat);
            FilterCollectionMeth();
        }
        internal void UpdateFlat(Flat flat)
        {
            Flat dbFlat = dataBase.Flats.Find(flat.Id);
            dbFlat = flat;
            dataBase.SaveChanges();
            Flat listFlat = (Flat)AllObjects.Find(f => f.Id == flat.Id && f.ObjectType == flat.ObjectType);
            listFlat = flat;
            FilterCollectionMeth();
            WriteLastUpdateTime();
        }
        internal void DeleteFlat(Flat flat)
        {
            Flat dbFlat = dataBase.Flats.Find(flat.Id);
            dataBase.Flats.Remove(dbFlat);
            dataBase.SaveChanges();
            Flat listFlat = (Flat)AllObjects.Find(f => f.Id == flat.Id && f.ObjectType == flat.ObjectType);
            AllObjects.Remove(listFlat);
            FilterCollectionMeth();
            WriteLastUpdateTime();
        }
        internal void AddHouse(House house)
        {
            dataBase.Houses.Add(house);
            dataBase.SaveChanges();
            WriteLastUpdateTime();
        }
        internal void UpdateHouse(House house)
        {
            House dbHouse = dataBase.Houses.Find(house.Id);
            dbHouse = house;
            dataBase.SaveChanges();
            WriteLastUpdateTime();
        }
        internal void DeleteHouse(House house)
        {
            House dbHouse = dataBase.Houses.Find(house.Id);
            dataBase.Houses.Remove(dbHouse);
            dataBase.SaveChanges();
            WriteLastUpdateTime();
        }

        private LocationOptions GetLocationOptions()
        {
            LocationOptions locationOptions = new LocationOptions();
            foreach (City city in dataBase.Cities.AsNoTracking())
                locationOptions.Cities.Add(city);
            foreach (District district in dataBase.Districts.AsNoTracking())
                locationOptions.Districts.Add(district);
            foreach (Street street in dataBase.Streets.AsNoTracking())
                locationOptions.Streets.Add(street);
            return locationOptions;
        }
        internal String GetLastUpdateTime()
        {
            if (dataBase.UpdateTime.Local.Count == 0)
                return "never";
            else
            {
                DateTime dateTime = dataBase.UpdateTime.Find("LastUpdateTime").DateTime.Date;
                //return dateTime.ToString();
                //return dateTime.ToString("M.d.yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                return dateTime.Date.ToString("M.d.yyyy", CultureInfo.InvariantCulture);
            }
            //return "never";
        }
        private void WriteLastUpdateTime()
        {
            if (dataBase.UpdateTime.Local.Count == 0)
                dataBase.UpdateTime.Local.Add(new UpdateTime() { DateTime = DateTime.Now.Date });
            else
            {
                UpdateTime lastUpdateTime = dataBase.UpdateTime.Find("LastUpdateTime");
                lastUpdateTime.DateTime = DateTime.Now.Date;
            }
            dataBase.SaveChanges();
        }
        private bool CheckAccess(string objectAgent, string currentAgent)
        {
            if (objectAgent == currentAgent)
            {
                return true;
            }
            else
            {
                MessageBox.Show("У вас нет права на доступ к этому объекту");
                return false;
            }
        }
    }
}
