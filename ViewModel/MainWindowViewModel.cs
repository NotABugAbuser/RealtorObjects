using FontAwesome.WPF;
using RealtorObjects.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using RealtyModel.Model;
using RealtyModel.Service;
using RealtorObjects.Model;
using Screen = System.Windows.Forms.Screen;
using FontAwesome5;
using RealtyModel.Model.Base;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;

namespace RealtorObjects.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private readonly string currentAgentName = "Пользователь 0";
        private readonly string[] streets;
        private double heightOfFilters = 690;
        private EFontAwesomeIcon filterArrow = EFontAwesomeIcon.Solid_AngleUp;
        private CustomCommand changeHeightOfFilters;
        private CustomCommand createFlat;
        private CustomCommand createHouse;
        private AsyncCommand modify;
        private List<ObservableCollection<BaseRealtorObject>> pages = new List<ObservableCollection<BaseRealtorObject>>() { };
        private string[] pageButtons;
        private ObservableCollection<BaseRealtorObject> currentPage = new ObservableCollection<BaseRealtorObject>();
        private Int16 currentPageNumber = 1;
        private CustomCommand requestRealtorObjects;
        private Filter filter = new Filter();
        public MainWindowViewModel() {
        }
        public MainWindowViewModel(string agentName) {
            this.currentAgentName = agentName;
            this.streets = Client.RequestStreets();
        }
        public CustomCommand RequestRealtorObjects => requestRealtorObjects ?? (requestRealtorObjects = new CustomCommand(obj => {
            List<BaseRealtorObject> realtorObjects = Client.RequestRealtorObjects(new Filter());
            pages = Pagination.Split(realtorObjects);
            CurrentPageNumber = 1;
            PageButtons = Pagination.Paginate(pages.Count, CurrentPageNumber);
            CurrentPage = pages[CurrentPageNumber - 1];
        }));
        public CustomCommand CreateFlat => createFlat ?? (createFlat = new CustomCommand(obj => {
            string objectType = obj as string;
            FlatFormViewModel flatFormVM = new FlatFormViewModel(CurrentAgentName, objectType);
            flatFormVM.Streets = this.streets;
            new FlatFormV3(flatFormVM).Show();
        }));
        public CustomCommand CreateHouse => createHouse ?? (createHouse = new CustomCommand(obj => {
            string objectType = obj as string;
            HouseFormViewModel houseFormVM = new HouseFormViewModel(CurrentAgentName, objectType);
            houseFormVM.Streets = this.streets;
            new HouseFormV2(houseFormVM).Show();
        }));
        public CustomCommand ChangeHeightOfFilters => changeHeightOfFilters ?? (changeHeightOfFilters = new CustomCommand(obj => {
            if (HeightOfFilters == 50) {
                HeightOfFilters = 690;
                FilterArrow = EFontAwesomeIcon.Solid_AngleDown;
            } else {
                HeightOfFilters = 50;
                FilterArrow = EFontAwesomeIcon.Solid_AngleUp;
            }
        }));
        public Int16 CurrentPageNumber {
            get => currentPageNumber;
            set {
                currentPageNumber = value;
                OnPropertyChanged();
            }
        }
        public AsyncCommand Modify => modify ?? (modify = new AsyncCommand(() => {
            BaseRealtorObject bro = (BaseRealtorObject)Modify.Parameter;
            if (bro is Flat flat) {
                return ModifyFlat(flat);
            } else if (bro is House house) {
                return ModifyHouse(house);
            } else {
                return Task.Run(() => {
                    OperationNotification.Notify(ErrorCode.Unknown);
                });
            }
        }));
        private Task ModifyFlat(Flat flat) {
            FlatFormViewModel flatFormVM = new FlatFormViewModel();
            Application.Current.Dispatcher.Invoke(() => {
                flat.Album = Client.RequestAlbum(flat.AlbumId);
                flatFormVM = new FlatFormViewModel(flat, CurrentAgentName);
                flatFormVM.Streets = this.streets;
                new FlatFormV3(flatFormVM).Show();
            });
            return Task.Run(() => {
                flatFormVM.Photos = BinarySerializer.Deserialize<ObservableCollection<byte[]>>(flatFormVM.OriginalFlat.Album.PhotoCollection);
                flatFormVM.CurrentImage = flatFormVM.Photos[0];
            });
        }
        private Task ModifyHouse(House house) {
            HouseFormViewModel houseFormVM = new HouseFormViewModel();
            Application.Current.Dispatcher.Invoke(() => {
                house.Album = Client.RequestAlbum(house.AlbumId);
                houseFormVM = new HouseFormViewModel(house, CurrentAgentName);
                houseFormVM.Streets = this.streets;
                new HouseFormV2(houseFormVM).Show();
            });
            return Task.Run(() => {
                houseFormVM.Photos = BinarySerializer.Deserialize<ObservableCollection<byte[]>>(houseFormVM.OriginalHouse.Album.PhotoCollection);
                houseFormVM.CurrentImage = houseFormVM.Photos[0];
            });
        }
        public string CurrentAgentName => currentAgentName;
        public double HeightOfFilters {
            get => heightOfFilters;
            set {
                heightOfFilters = value;
                OnPropertyChanged();
            }
        }
        public EFontAwesomeIcon FilterArrow {
            get => filterArrow;
            set {
                filterArrow = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<BaseRealtorObject> CurrentPage {
            get => currentPage;
            set {
                currentPage = value;
                OnPropertyChanged();
            }
        }

        public string[] PageButtons {
            get => pageButtons;
            set {
                pageButtons = value;
                OnPropertyChanged();
            }
        }

        public Filter Filter {
            get => filter; set => filter = value;
        }
    }
}
