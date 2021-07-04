using RealtorObjects.ViewModel;
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
    /// Логика взаимодействия для FlatFormV2.xaml
    /// </summary>
    public partial class FlatFormV2 : Window
    {
        public FlatFormV2()
        {
            InitializeComponent();
        }
        public FlatFormV2(FlatFormViewModel flatFormVM)
        {
            InitializeComponent();
            this.DataContext = flatFormVM;
        }
        
        private void CloseApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void MinimizeApplcation_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        private void WindowApplication_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;

            }
        }
        
        private void EnglishOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^a-zA-z]").IsMatch(e.Text);
        }
        private void RussianOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^а-яА-Я]").IsMatch(e.Text);
        }
        private void RussianWithNumbersOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^а-яА-Я0-9-]").IsMatch(e.Text);
        }
        private void NumericOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]").IsMatch(e.Text);
        }
        private void AlphabeticalOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^а-яА-Яa-zA-z]").IsMatch(e.Text);
        }
        private void AnyLetter(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^а-яА-Яa-zA-z0-9,]").IsMatch(e.Text);
        }
        private void PhoneNumbers(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9;,]").IsMatch(e.Text);
        }
        private void NumericWithDotOnly(object sender, TextCompositionEventArgs e)
        {
                if (new Regex("[0-9]").IsMatch(e.Text))
                    e.Handled = false;
                else if (new Regex(@"\.").IsMatch(e.Text))
                {
                    if ((sender as TextBox).Text.Contains("."))
                        e.Handled = true;
                    else e.Handled = false;
                }
                else e.Handled = true;
        }
        private void NumericWithCommaOnly(object sender, TextCompositionEventArgs e)
        {
            if ((sender as TextBox).Text.Length < 6)
            {
                if (new Regex("[0-9]").IsMatch(e.Text))
                    e.Handled = false;
                else if (new Regex(@"\,").IsMatch(e.Text))
                {
                    if ((sender as TextBox).Text.Contains(","))
                        e.Handled = true;
                    else e.Handled = false;
                }
                else e.Handled = true;
            }
            else e.Handled = true;
        }
    }
}
