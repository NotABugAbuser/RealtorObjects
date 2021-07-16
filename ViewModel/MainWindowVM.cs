using FontAwesome.WPF;
using RealtorObjects.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using RealtyModel.Model;
using RealtorObjects.Model;
using Screen = System.Windows.Forms.Screen;
using FontAwesome5;
using RealtyModel.Model.Base;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;
using RealtyModel.Model.Tools;

namespace RealtorObjects.ViewModel
{
    public class MainWindowVM : BaseVM
    {
        private readonly string currentAgentName = "Пользователь 0";
        private readonly int currentAgentId = 0;
        private readonly string[] streets;
        private EFontAwesomeIcon filtrationArrow = EFontAwesomeIcon.Solid_AngleUp;
        private CustomCommand createFlat;
        private CustomCommand createHouse;
        private CustomCommand logOut;
        private CustomCommand openAccounts;
        private AsyncCommand goToPageAsync;
        private AsyncCommand modify;
        private List<ObservableCollection<BaseRealtorObject>> pages = new List<ObservableCollection<BaseRealtorObject>>() { };
        private string[] pageButtons = { "1" };
        private ObservableCollection<BaseRealtorObject> currentPage = new ObservableCollection<BaseRealtorObject>();
        private Int32 currentPageNumber = 1;
        private CustomCommand requestRealtorObjects;
        private Filtration filtration = new Filtration();
        public MainWindowVM() {
        }
        public MainWindowVM(string agentName, int code) {
            this.currentAgentName = agentName;
            this.currentAgentId = code;
            this.streets = Client.RequestStreets();
        }
        public CustomCommand OpenAccounts => openAccounts ?? (openAccounts = new CustomCommand(obj => {
            List<Agent> agents = Client.RequestAgents();
            new Accounts(new AccountsVM(agents)).ShowDialog();
        }));
        public CustomCommand LogOut => logOut ?? (logOut = new CustomCommand(obj => {
            Window window = obj as Window;
            new LoginFormV2(new LoginFormVM()).Show();
            window.Close();
        }));
        public AsyncCommand GoToPageAsync => goToPageAsync ?? (goToPageAsync = new AsyncCommand(() => {
            return Task.Run(() => {
                string page = goToPageAsync.Parameter as string;
                if (page == "<") {
                    CurrentPageNumber -= 1;
                    CurrentPage = pages[CurrentPageNumber - 1];
                    PageButtons = Pagination.Paginate(pages.Count, CurrentPageNumber);
                } else if (page == ">") {
                    CurrentPageNumber += 1;
                    CurrentPage = pages[CurrentPageNumber - 1];
                    PageButtons = Pagination.Paginate(pages.Count, CurrentPageNumber);
                } else if (page != "...") {
                    Int32 selectedPage = Convert.ToInt32(page);
                    CurrentPageNumber = selectedPage;
                    CurrentPage = pages[selectedPage - 1];
                    PageButtons = Pagination.Paginate(pages.Count, CurrentPageNumber);
                }
            });
        }));
        public CustomCommand RequestRealtorObjects => requestRealtorObjects ?? (requestRealtorObjects = new CustomCommand(obj => {
            List<BaseRealtorObject> realtorObjects = Client.RequestRealtorObjects(filtration);
            pages = Pagination.Split(realtorObjects);
            CurrentPageNumber = 1;
            PageButtons = Pagination.Paginate(pages.Count, CurrentPageNumber);
            CurrentPage = pages[CurrentPageNumber - 1];
        }));
        public CustomCommand CreateFlat => createFlat ?? (createFlat = new CustomCommand(obj => {
            FlatFormVM flatFormVM = new FlatFormVM(CurrentAgentName, currentAgentId);
            flatFormVM.Streets = this.streets;
            new FlatFormV3(flatFormVM).Show();
        }));
        public CustomCommand CreateHouse => createHouse ?? (createHouse = new CustomCommand(obj => {
            HouseFormVM houseFormVM = new HouseFormVM(CurrentAgentName, currentAgentId);
            houseFormVM.Streets = this.streets;
            new HouseFormV2(houseFormVM).Show();
        }));
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
            FlatFormVM flatFormVM = new FlatFormVM();
            Application.Current.Dispatcher.Invoke(() => {
                flat.Album = Client.RequestAlbum(flat.AlbumId);
                flatFormVM = new FlatFormVM(flat, CurrentAgentName, currentAgentId);
                flatFormVM.Streets = this.streets;
                new FlatFormV3(flatFormVM).Show();
            });
            return Task.Run(() => {
                flatFormVM.Photos = BinarySerializer.Deserialize<ObservableCollection<byte[]>>(flatFormVM.OriginalFlat.Album.PhotoCollection);
                flatFormVM.CurrentImage = flatFormVM.Photos[0];
            });
        }
        private Task ModifyHouse(House house) {
            HouseFormVM houseFormVM = new HouseFormVM();
            Application.Current.Dispatcher.Invoke(() => {
                house.Album = Client.RequestAlbum(house.AlbumId);
                houseFormVM = new HouseFormVM(house, CurrentAgentName, currentAgentId);
                houseFormVM.Streets = this.streets;
                new HouseFormV2(houseFormVM).Show();
            });
            return Task.Run(() => {
                houseFormVM.Photos = BinarySerializer.Deserialize<ObservableCollection<byte[]>>(houseFormVM.OriginalHouse.Album.PhotoCollection);
                houseFormVM.CurrentImage = houseFormVM.Photos[0];
            });
        }
        public string CurrentAgentName => currentAgentName;
        public EFontAwesomeIcon FiltrationArrow {
            get => filtrationArrow;
            set {
                filtrationArrow = value;
                OnPropertyChanged();
            }
        }
        public Int32 CurrentPageNumber {
            get => currentPageNumber;
            set {
                currentPageNumber = value;
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

        public Filtration Filtration {
            get => filtration; set => filtration = value;
        }
    }
}
