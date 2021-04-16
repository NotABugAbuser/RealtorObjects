using RandomFlatGenerator;
using RealtorObjects.Model;
using RealtorObjects.View;
using RealtyModel.Event;
using RealtyModel.Event.RealtyEvents;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        public event OpeningFlatFormEventHandler OpeningFlatForm;
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
                    OpeningFlatForm?.Invoke(this, new OpeningFlatFormEventArgs(true, new Flat()));
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
            FlatGenerator flatGenerator = new FlatGenerator();
            foreach (BaseRealtorObject bro in flatGenerator.CreateFlatList(AllObjects.Count, AllObjects.Count + 50))
            {
                bro.Agent = "ГвоздиковЕА";
                AllObjects.Add(bro);
            }
        }));
        public CustomCommand FilterCollection => filterCollection ?? (filterCollection = new CustomCommand(obj =>
        {
            List<BaseRealtorObject> filteredObjects = Filter.CreateFilteredList(AllObjects);
            SplitFilteredCollection(filteredObjects, 25);
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

        public void OnAddHouse()
        {

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
        public void ReceiveUpdate(ReceivedObjectDBEventArgs e)
        {
            if(e.TargetType == TargetType.Flat)
            {
                Flat[] flats = JsonSerializer.Deserialize<Flat[]>((String)e.UpdateData);
                allObjects = new List<BaseRealtorObject>(flats);
            }
        }

        private ObservableCollection<BaseRealtorObject> currentObjectList = new ObservableCollection<BaseRealtorObject>() { };
        private List<ObservableCollection<BaseRealtorObject>> objectLists = new List<ObservableCollection<BaseRealtorObject>>();
        private List<BaseRealtorObject> allObjects = new List<BaseRealtorObject>();
        private CustomCommand openOrCloseFilterSection;
        private CustomCommand testCommand;
        private CustomCommand filterCollection;
        private CustomCommand delete;
        private CustomCommand modify;
        private CustomCommand createRealtorObject;
        private CustomCommand openCloseFilters;
        private Filter filter = new Filter();
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
        private double widthOfFilters = 200;

        public HomeViewModel()
        {
            //RealtorObjectOperator.Client = this.Client;
            TestMethod();
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
        public List<ObservableCollection<BaseRealtorObject>> ObjectLists
        {
            get => objectLists;
            set => objectLists = value;
        }
        public List<BaseRealtorObject> AllObjects
        {
            get => allObjects;
            set => allObjects = value;
        }
        public ObservableCollection<CheckAndHeightPair> FilterAreaSections => filterAreaSections;
        public Filter Filter
        {
            get => filter;
            set => filter = value;
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
    }
}
