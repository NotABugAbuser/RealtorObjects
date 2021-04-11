using RandomFlatGenerator;
using RealtorObjects.Model;
using RealtorObjects.View;
using RealtyModel.Event;
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
        private RealtorObjectOperator realtorObjectOperator = new RealtorObjectOperator();
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
        public HomeViewModel() {
            RealtorObjectOperator.Client = this.Client;
            TestMethod();
        }
        #region Используется для отладки, потом удалить
        public void HandleFlat(object sender, FlatCreatedEventArgs e) {
            Operation operation = new Operation(Client.CurrentAgent, JsonSerializer.Serialize(e.Flat), OperationDirection.Realty, OperationType.Add, TargetType.Flat);
            if (AllObjects.Where(x => x.Id == e.Flat.Id).Count() == 0) {
                AllObjects.Add(e.Flat);
                Client.SendMessage(operation);
            } else {
                operation.OperationParameters.Type = OperationType.Change;
            }
            Client.SendMessage(operation);
        }
        private void TestMethod() {
            FlatGenerator flatGenerator = new FlatGenerator();
            Flat flat = flatGenerator.CreateFlat();
            flat.Id = 9999;
            flat.Agent = "ГвоздиковЕА";
            flat.Status = Status.Planned;
            CurrentObjectList.Add(flat);
            AllObjects.Add(flat);
        }
        #endregion

        public CustomCommand OpenCloseFilters => openCloseFilters ?? (openCloseFilters = new CustomCommand(obj => {
            if (WidthOfFilters == 200) {
                WidthOfFilters = 0;
            } else {
                WidthOfFilters = 200;
            }
        }));
        public CustomCommand CreateRealtorObject => createRealtorObject ?? (createRealtorObject = new CustomCommand(obj => {
            string type = (string)obj;
            if (type == "House") RealtorObjectOperator.CreateHouse();
            if (type == "Flat") RealtorObjectOperator.CreateFlat();
            if (type == "Plot") RealtorObjectOperator.CreatePlot();
        }));
        public CustomCommand Modify => modify ?? (modify = new CustomCommand(obj => {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            if (CheckAccess(bro.Agent, Client.CurrentAgent)) {
                if (bro is Flat flat) RealtorObjectOperator.ModifyFlat(flat);
                if (bro is House house) RealtorObjectOperator.ModifyHouse(house);
                //добавить для Plot
            }
        }));
        public CustomCommand Delete => delete ?? (delete = new CustomCommand(obj => {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            if (CheckAccess(bro.Agent, Client.CurrentAgent)) {
                AllObjects.RemoveAll(x => x.Id == bro.Id);
                CurrentObjectList.Remove(bro);
                Operation operation = new Operation(Client.CurrentAgent, bro.Id.ToString(), OperationDirection.Realty, OperationType.Remove);
            }
        }));
        private bool CheckAccess(string objectAgent, string currentAgent) {
            if (objectAgent == currentAgent) {
                return true;
            } else {
                MessageBox.Show("У вас нет права на доступ к этому объекту");
                return false;
            }
        }
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj => {
            FlatGenerator flatGenerator = new FlatGenerator();
            foreach (BaseRealtorObject bro in flatGenerator.CreateFlatList(AllObjects.Count, AllObjects.Count + 50)) {
                bro.Agent = "ГвоздиковЕА";
                AllObjects.Add(bro);
            }
        }));
        public CustomCommand FilterCollection => filterCollection ?? (filterCollection = new CustomCommand(obj => {
            List<BaseRealtorObject> filteredObjects = Filter.CreateFilteredList(AllObjects);
            SplitFilteredCollection(filteredObjects, 25);
            GC.Collect();
        }));
        private void SplitFilteredCollection(List<BaseRealtorObject> filteredObjects, byte count) {
            //разбиение фильтрованной коллекции на parts частей по count объектов
            Int16 parts = (Int16)Math.Ceiling(filteredObjects.Count / (double)count);
            List<List<BaseRealtorObject>> lists = Enumerable.Range(0, parts).AsParallel().Select(x => filteredObjects.Skip(x * count).Take(count).ToList()).ToList();
            ObjectLists.Clear();
            foreach (List<BaseRealtorObject> ol in lists) {
                ObjectLists.Add(new ObservableCollection<BaseRealtorObject>(ol));
            }
            CurrentObjectList = ObjectLists[0];
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
        public ObservableCollection<CheckAndHeightPair> FilterAreaSections => filterAreaSections;
        public Filter Filter {
            get => filter;
            set => filter = value;
        }
        public RealtorObjectOperator RealtorObjectOperator {
            get => realtorObjectOperator;
            set => realtorObjectOperator = value;
        }
        public double WidthOfFilters {
            get => widthOfFilters;
            set {
                widthOfFilters = value;
                OnPropertyChanged();
            }
        }
    }
}
