using RealtorObjects.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RealtyModel.Service;

namespace RealtorObjects.View
{
    /// <summary>
    /// Логика взаимодействия для LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public LoginForm() {
            InitializeComponent();
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
            
        }
        private void LoginValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^а-яА-Я]").IsMatch(e.Text);
        }
        private void EmailValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^a-zA-Z0-9@.]").IsMatch(e.Text);
        }
        private void PasswordValidation(object sender, TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9a-zA-Z]").IsMatch(e.Text);
        }
    }
}
