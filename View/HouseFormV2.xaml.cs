using RealtorObjects.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RealtorObjects.View
{
    /// <summary>
    /// Логика взаимодействия для HouseFormV2.xaml
    /// </summary>
    public partial class HouseFormV2 : Window
    {
        public HouseFormV2() {
            InitializeComponent();
        }
        public HouseFormV2(HouseFormVM houseFormVM) {
            InitializeComponent();
            this.DataContext = houseFormVM;
        }
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
        private void EnglishOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^a-zA-z]").IsMatch(e.Text);
        }
        private void RussianOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^а-яА-Я]").IsMatch(e.Text);
        }
        private void RussianWithNumbersOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^а-яА-Я0-9-]").IsMatch(e.Text);
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
        private void NumericWithDotOnly(object sender, TextCompositionEventArgs e) {
        }
        private void TestRegex(object sender, TextCompositionEventArgs e) {
            e.Handled = (new Regex(@"(?<= ^| )\d + (\.\d +) ? (?=$| )").IsMatch(e.Text));
        }
        private void NumericWithCommaOnly(object sender, TextCompositionEventArgs e) {
            if ((sender as TextBox).Text.Length < 6) {
                if (new Regex("[0-9]").IsMatch(e.Text))
                    e.Handled = false;
                else if (new Regex(@"\,").IsMatch(e.Text)) {
                    if ((sender as TextBox).Text.Contains(","))
                        e.Handled = true;
                    else e.Handled = false;
                } else e.Handled = true;
            } else e.Handled = true;
        }
    }
}
