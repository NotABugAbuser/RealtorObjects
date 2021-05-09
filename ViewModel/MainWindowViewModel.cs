using FontAwesome.WPF;
using RealtorObjects.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using RealtyModel.Model;
using RealtyModel.Service;

namespace RealtorObjects.ViewModel
{
    public class MainWindowViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string currentTime;
        private string currentAgentName = "ПроверкинПП";
        private Credential credential;
        private BaseViewModel workArea;
        private CustomCommand closeApp;
        private CustomCommand updateWorkArea;
        private CustomCommand signOut;
        private FontAwesomeIcon currentIcon = FontAwesomeIcon.Home;
        private FontAwesomeIcon[] icons = new FontAwesomeIcon[5] {
            FontAwesomeIcon.Home,
            FontAwesomeIcon.Phone,
            FontAwesomeIcon.BarChart,
            FontAwesomeIcon.List,
            FontAwesomeIcon.AddressBook,
        };
        private LocationOptions locationOptions = new LocationOptions();
        private BaseViewModel[] viewModels = new BaseViewModel[6];
        readonly ObservableCollection<bool> toggledButtons = new ObservableCollection<bool> {
            true,
            false,
            false,
            false,
            false,
            false
        };

        public string CurrentAgentName {
            get => currentAgentName;
            set {
                currentAgentName = value;
                OnPropertyChanged();
            }
        }
        public string CurrentTime {
            get => currentTime;
            set {
                currentTime = value;
                OnPropertyChanged();
            }
        }
        public FontAwesomeIcon CurrentIcon {
            get => currentIcon;
            set {
                currentIcon = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<bool> ToggledButtons => toggledButtons;
        public LocationOptions LocationOptions {
            get => locationOptions;
            set {
                locationOptions = value;
                OnPropertyChanged();
            }
        }
        public BaseViewModel WorkArea {
            get => workArea;
            set {
                workArea = value;
                OnPropertyChanged();
            }
        }
        public BaseViewModel[] ViewModels {
            get => viewModels;
            set => viewModels = value;
        }

        public MainWindowViewModel(Credential credential) {
            this.credential = credential;
            //StartUpTheClock();
        }
        
        private CustomCommand testCommand;
        private CustomCommand secondTestCommand;
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj => {
            var flatWindow = new FlatFormV2();
            flatWindow.Show();
        }));
        public CustomCommand SecondTestCommand => secondTestCommand ?? (secondTestCommand = new CustomCommand(obj => {
        }));

        public CustomCommand SignOut => signOut ?? (signOut = new CustomCommand(obj => {
            credential.OnLoggedOut();
        }));
        public CustomCommand CloseApp => closeApp ?? (closeApp = new CustomCommand(obj => {
            Application.Current.Shutdown();
        }));
        public CustomCommand UpdateWorkArea => updateWorkArea ?? (updateWorkArea = new CustomCommand(obj => {
            byte index = Convert.ToByte(obj);
            WorkArea = ViewModels[index];
            for (byte i = 0; i < ToggledButtons.Count; i++) {
                ToggledButtons[i] = false;
            }
            ToggledButtons[index] = true;
        }));
    }
}
