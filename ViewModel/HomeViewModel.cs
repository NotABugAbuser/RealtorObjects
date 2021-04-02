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
    class HomeViewModel : BaseViewModel
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
        public HomeViewModel() {
            FlatGenerator flatGenerator = new FlatGenerator();
            Flat flat = flatGenerator.CreateFlat();
            flat.Id = 9999;
            flat.Agent = "ГвоздиковЕА";
            flat.Status = Status.Archived;
            CurrentObjectList.Add(flat);
            AllObjects.Add(flat);
            FlatFormViewModel flatFormVM = ((App)Application.Current).FlatFormViewModel;
            flatFormVM.FlatCreated += HandleFlat;
        }
        public void HandleHouse(object sender, FlatCreatedEventArgs e) {

        }
        public void HandlePlot(object sender, FlatCreatedEventArgs e) {

        }
        public void HandleFlat(object sender, FlatCreatedEventArgs e) {
            Client client = ((App)Application.Current).Client;
            if (AllObjects.Where(x => x.Id == e.Flat.Id).Count() == 0) {
                Debug.WriteLine("Таких объектов нет");
                Operation operation = new Operation(client.CurrentAgent, JsonSerializer.Serialize(e.Flat), OperationDirection.Realty, OperationType.Add);
                AllObjects.Add(e.Flat);
                //отправить операцию на сервер
            } else {
                Operation operation = new Operation(client.CurrentAgent, JsonSerializer.Serialize(e.Flat), OperationDirection.Realty, OperationType.Change);
                Debug.WriteLine($"Номер объекта {AllObjects.FindIndex(x => x.Id == e.Flat.Id)}");
                AllObjects[AllObjects.FindIndex(x => x.Id == e.Flat.Id)] = e.Flat;
                //отправить операцию на сервер
            }
        }
        public CustomCommand CreateRealtorObject => createRealtorObject ?? (createRealtorObject = new CustomCommand(obj => {
            string type = (string)obj;
            Client client = ((App)Application.Current).Client;
            if (type == "House") {

            }
            if (type == "Flat") {
                Flat flat = new Flat(true) { Agent = client.CurrentAgent};
                FlatFormV2 flatForm = new FlatFormV2();
                FlatFormViewModel flatFormVM = ((App)Application.Current).FlatFormViewModel;
                flatFormVM.Title = "[Квартира] — Создание";
                flatFormVM.Flat = flat;
                flatFormVM.LocationOptions = new LocationOptions();
                flatForm.DataContext = flatFormVM;
                flatForm.Show();
            }
            if (type == "Plot") {

            }
        }));
        public CustomCommand Modify => modify ?? (modify = new CustomCommand(obj => {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            Client client = ((App)Application.Current).Client;
            if (CheckAccess(bro.Agent, client.CurrentAgent)) {
                if (bro is Flat flat) {
                    Operation operation = new Operation(client.CurrentAgent, JsonSerializer.Serialize(flat), OperationDirection.Realty, OperationType.Change);
                    FlatFormViewModel flatFormVM = ((App)Application.Current).FlatFormViewModel;
                    flatFormVM.Title = "[Квартира] — Редактирование";
                    flatFormVM.Flat = JsonSerializer.Deserialize<Flat>(JsonSerializer.Serialize(flat)); //нужно, чтобы разорвать связь объекта в форме и объекта в списке
                    flatFormVM.LocationOptions = new LocationOptions();
                    new FlatFormV2 { DataContext = flatFormVM }.Show();
                    // отправить операцию на сервер
                } else if (bro is House) {

                }
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
        public CustomCommand Delete => delete ?? (delete = new CustomCommand(obj => {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            Client client = ((App)Application.Current).Client;
            if (CheckAccess(bro.Agent, client.CurrentAgent)) {
                AllObjects.RemoveAll(x => x.Id == bro.Id);
                CurrentObjectList.Remove(bro);
                Operation operation = new Operation(client.CurrentAgent, "", OperationDirection.Realty, OperationType.Remove);
                //вот тут логика отправки запроса удаления на сервер
            }
        }));
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
    }
}
