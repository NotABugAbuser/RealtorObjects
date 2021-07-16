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
    /// Логика взаимодействия для Accounts.xaml
    /// </summary>
    public partial class Accounts : Window
    {
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
        public Accounts() {
            InitializeComponent();
        }
        public Accounts(AccountsVM accountsVM) {
            InitializeComponent();
            this.DataContext = accountsVM;
        }
        private void Names(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^а-яА-Я]").IsMatch(e.Text);
        }
        private void NumbersOnly(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9]").IsMatch(e.Text);
        }
        private void Passwords(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9a-zA-Z]").IsMatch(e.Text);
        }
        private void Emails(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9a-zA-Z@.]").IsMatch(e.Text);
        }

    }
}
