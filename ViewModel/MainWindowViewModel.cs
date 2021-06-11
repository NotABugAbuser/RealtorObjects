using FontAwesome.WPF;
using RealtorObjects.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using RealtyModel.Model;
using RealtyModel.Service;

namespace RealtorObjects.ViewModel
{
    public class MainWindowViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string currentAgentName = "Пользователь 0";
        private BaseViewModel workArea;
        private CustomCommand closeApp;
        private CustomCommand updateWorkArea;
        private CustomCommand signOut;
        private CustomCommand openRegistrationForm;
        private Street[] streets = Array.Empty<Street>();
        private BaseViewModel[] viewModels = new BaseViewModel[6] {
            new HomeViewModel(),
            new BaseViewModel(),
            new BaseViewModel(),
            new BaseViewModel(),
            new BaseViewModel(),
            new BaseViewModel(),
        };
        readonly ObservableCollection<bool> toggledButtons = new ObservableCollection<bool> {
            true,
            false,
            false,
            false,
            false,
            false
        };
        public CustomCommand OpenRegistrationForm => openRegistrationForm ?? (openRegistrationForm = new CustomCommand(obj => {
            new RegistrationForm() { DataContext = new RegistrationFormViewModel() }.Show();
        }));
        public string CurrentAgentName {
            get => currentAgentName;
            set {
                currentAgentName = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<bool> ToggledButtons => toggledButtons;
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
        public Street[] Streets{
            get => streets;
            set {
                streets = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel() {
            CurrentAgentName = (Application.Current as App).AgentName;
            WorkArea = viewModels[0];
        }

        public CustomCommand SignOut => signOut ?? (signOut = new CustomCommand(obj => {
            Window window = obj as Window;
            window.Close();
            new LoginForm() { DataContext = new LoginFormViewModel() }.Show();
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
