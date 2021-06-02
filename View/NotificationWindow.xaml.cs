using FontAwesome.WPF;
using RealtorObjects.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace RealtorObjects.View
{
    /// <summary>
    /// Логика взаимодействия для NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window, INotifyPropertyChanged
    {
        private NotificationInfo notificationInfo = new NotificationInfo("Запрашиваемая информация отсутствует в базе данных", CodeType.Exclamation);
        public NotificationWindow() {
            InitializeComponent();
            DataContext = this;
        }
        public NotificationWindow(NotificationInfo notificationInfo) {
            InitializeComponent();
            this.NotificationInfo = notificationInfo;
            DataContext = this;
            StartCloseTimer();
        }
        private void StartCloseTimer() {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(5d);
            timer.Tick += TimerTick;
            timer.Start();
        }

        private void TimerTick(object sender, EventArgs e) {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            Close();
        }
        public NotificationInfo NotificationInfo {
            get => notificationInfo;
            set {
                notificationInfo = value;
                OnPropertyChanged();
            }
        }
        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
