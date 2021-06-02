using RealtorObjects.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RealtorObjects.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindowV2.xaml
    /// </summary>
    public partial class MainWindowV2 : Window
    {
        public MainWindowV2() {
            InitializeComponent();
        }
        public MainWindowV2(MainWindowViewModel mainWindowVM) {
            InitializeComponent();
            this.DataContext = mainWindowVM;
        }
        private void CloseApplication_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
        private void MinimizeApplcation_Click(object sender, RoutedEventArgs e) {
            this.WindowState = WindowState.Minimized;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
            base.OnClosing(e);
            Application.Current.Shutdown();
        }
        private void ThisMainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }
        private void WindowApplication_Click(object sender, RoutedEventArgs e) {
            if (this.WindowState == WindowState.Maximized) {
                this.WindowState = WindowState.Normal;
            } else {
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}
