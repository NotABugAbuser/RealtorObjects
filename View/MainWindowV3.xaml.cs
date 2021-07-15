using RealtorObjects.Model;
using RealtorObjects.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Screen = System.Windows.Forms.Screen;

namespace RealtorObjects.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindowV3.xaml
    /// </summary>
    public partial class MainWindowV3 : Window
    {
        public MainWindowV3() {
            InitializeComponent();
        }
        public MainWindowV3(MainWindowVM mainWindowVM) {
            InitializeComponent();
            this.DataContext = mainWindowVM;
        }
    }
}
