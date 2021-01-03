using FontAwesome.WPF;
using RealtorObjects.Model;
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
using System.Threading;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private CustomCommand testCommand;
        private BaseViewModel workAreaViewModel = new RealtorObjectsViewModel();
        private CustomCommand secondTestCommand;
        private CustomCommand updateWorkAreaViewModel;
        private string header = "Главная";
        private FontAwesomeIcon currentIcon = FontAwesomeIcon.Home;
        private BaseViewModel[] viewModels = new BaseViewModel[] {
            new HomeViewModel(),
            new PhoneNumbersViewModel(),
            new StatisticsViewModel(),
            new RealtorObjectsViewModel(),
            new CustomersViewModel()
        };
        private FontAwesomeIcon[] icons = new FontAwesomeIcon[5] {
            FontAwesomeIcon.Home,
            FontAwesomeIcon.Phone,
            FontAwesomeIcon.BarChart,
            FontAwesomeIcon.List,
            FontAwesomeIcon.AddressBook,
        };
        ObservableCollection<bool> toggledButtons = new ObservableCollection<bool> {
            true,
            false,
            false,
            false,
            false
        };
        private string[] headers = new string[5]{
            "Главная",
            "Номера телефонов",
            "Статистика",
            "Объекты",
            "Клиенты",
        };
        private string currentTime;
        public MainWindowViewModel() {
            string dayOfWeek = new CultureInfo("ru-RU").DateTimeFormat.GetShortestDayName(DateTime.Now.DayOfWeek);
            CurrentTime = $"{DateTime.Now:HH : mm} {dayOfWeek}";
            Task.Factory.StartNew(() => {
                while (true) {
                    Thread.Sleep(5000);
                    CurrentTime = $"{DateTime.Now:HH:mm} {dayOfWeek}";
                }
            });
        }
        public CustomCommand UpdateWorkAreaViewModel => updateWorkAreaViewModel ?? (updateWorkAreaViewModel = new CustomCommand(obj => {
            byte index = Convert.ToByte(obj);
            WorkAreaViewModel = viewModels[index];
            Header = headers[index];
            CurrentIcon = icons[index];
            for (byte i = 0; i < ToggledButtons.Count; i++) {
                ToggledButtons[i] = false;
            }
            ToggledButtons[index] = true;
        }));
        public CustomCommand TestCommand => testCommand ?? (testCommand = new CustomCommand(obj => {
            var flatWindow = new FlatForm();
            flatWindow.Show();
        }));
        public CustomCommand SecondTestCommand => secondTestCommand ?? (secondTestCommand = new CustomCommand(obj => {
        }));
        public BaseViewModel WorkAreaViewModel {
            get => workAreaViewModel;
            set {
                workAreaViewModel = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<bool> ToggledButtons => toggledButtons;

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

        public string Header {
            get => header;
            set {
                header = value;
                OnPropertyChanged();
            }
        }

        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
