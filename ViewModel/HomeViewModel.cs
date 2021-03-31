using RandomFlatGenerator;
using RealtorObjects.Model;
using RealtorObjects.View;
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
        private ObservableCollection<BaseRealtorObject> currentObjectList = new ObservableCollection<BaseRealtorObject>() { };
        private List<ObservableCollection<BaseRealtorObject>> objectLists = new List<ObservableCollection<BaseRealtorObject>>();
        private List<BaseRealtorObject> allObjects = new List<BaseRealtorObject>();
        private CustomCommand openOrCloseFilterSection;
        private CustomCommand testCommand;
        private CustomCommand filterCollection;
        private CustomCommand delete;
        private CustomCommand modify;
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
            flat.Agent = "ГвоздиковЕА";
            flat.Status = Status.Archived;
            CurrentObjectList.Add(flat);
        }
        public CustomCommand Modify => modify ?? (modify = new CustomCommand(obj => {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            if (CheckAccess(bro.Agent)) {
                if (bro is Flat flat) {
                    Operation operation = new Operation() { };
                    FlatFormViewModel flatFormVM = new FlatFormViewModel(flat, "[Квартира] — Редактирование", new LocationOptions());
                    new FlatFormV2 { DataContext = flatFormVM }.Show();
                } else if (bro is House) {

                }
            }
        }));
        private bool CheckAccess(string agent) {
            Client client = ((App)Application.Current).Client;
            if (agent == "ГвоздиковЕА") {  //переделать на проверку агента из клиента
                return true;
            } else {
                MessageBox.Show("У вас нет права на доступ к этому объекту");
                return false;
            }
        }
        public CustomCommand Delete => delete ?? (delete = new CustomCommand(obj => {
            BaseRealtorObject bro = (BaseRealtorObject)obj;
            if (CheckAccess(bro.Agent)){
                AllObjects.RemoveAll(x => x.Id == bro.Id);
                CurrentObjectList.Remove(bro);
                Operation operation = new Operation() {
                    Name = "",
                    Data = "",
                    OperationParameters = new OperationParameters() {
                        Direction = OperationDirection.Realty,
                        Type = OperationType.Remove
                    },
                };
                Client client = ((App)Application.Current).Client;
                //вот тут логика отправки запроса удаления на сервер
            }
        }));
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj => {
            FlatGenerator flatGenerator = new FlatGenerator();
            foreach (BaseRealtorObject bro in flatGenerator.CreateFlatList(AllObjects.Count, AllObjects.Count + 50)) {
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
