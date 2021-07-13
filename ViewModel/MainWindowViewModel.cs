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

namespace RealtorObjects.ViewModel
{
    public class MainWindowViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string currentAgentName = "Пользователь 0";
        readonly private Street[] streets = Client.RequestStreets();
        private double GetActualWorkAreaSize() {
            return Screen.PrimaryScreen.Bounds.Height - Screen.PrimaryScreen.WorkingArea.Height - SystemParameters.WindowCaptionHeight;
        }
        public string CurrentAgentName {
            get => currentAgentName;
            set {
                currentAgentName = value;
                OnPropertyChanged();
            }
        }
        public MainWindowViewModel() {
            CurrentAgentName = (Application.Current as App).AgentName;
        }
    }
}
