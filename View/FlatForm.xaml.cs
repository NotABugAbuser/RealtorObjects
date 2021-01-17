using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using RealtorObjects.ViewModel;

namespace RealtorObjects.View
{
    /// <summary>
    /// Логика взаимодействия для FlatForm.xaml
    /// </summary>
    public partial class FlatForm : Window
    {
        public FlatForm() {
            InitializeComponent();
            DataContext = new FlatFormViewModel();
        }
        public FlatForm(FlatFormViewModel dataContext) {
            InitializeComponent();
            DataContext = dataContext;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }
        private void AlphabeticalOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^а-яА-Яa-zA-z]").IsMatch(e.Text);
        }
        private void NumericOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9]").IsMatch(e.Text);
        }

        private void PhoneNumbers(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9;,]").IsMatch(e.Text);
        }
    }
}
