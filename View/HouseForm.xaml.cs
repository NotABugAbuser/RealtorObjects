using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для HouseForm.xaml
    /// </summary>
    public partial class HouseForm : Window
    {
        public HouseForm() {
            InitializeComponent();
        }
        private void CloseApplication_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void MinimizeApplcation_Click(object sender, RoutedEventArgs e) {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void WindowApplication_Click(object sender, RoutedEventArgs e) {
            if (this.WindowState == WindowState.Normal) {
                this.WindowState = WindowState.Maximized;
            } else {
                this.WindowState = WindowState.Normal;

            }
        }
        private void EnglishOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^a-zA-z]").IsMatch(e.Text);
        }
        private void RussianOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^а-яА-Я]").IsMatch(e.Text);
        }
        private void NumericOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9]").IsMatch(e.Text);
        }
        private void AlphabeticalOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^а-яА-Яa-zA-z]").IsMatch(e.Text);
        }
        private void AnyLetter(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^а-яА-Яa-zA-z0-9,]").IsMatch(e.Text);
        }
        private void PhoneNumbers(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9;,]").IsMatch(e.Text);
        }
    }
}
