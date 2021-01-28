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
    /// Логика взаимодействия для FlatFormV2.xaml
    /// </summary>
    public partial class FlatFormV2 : Window
    {
        public FlatFormV2() {
            InitializeComponent();
        }
        private void CloseApplication_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void MinimizeApplcation_Click(object sender, RoutedEventArgs e) {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }

        private void WindowApplication_Click(object sender, RoutedEventArgs e) {
            if (this.WindowState == WindowState.Normal) {
                this.WindowState = WindowState.Maximized;
            } else {
                this.WindowState = WindowState.Normal;

            }
        }
    }
}
