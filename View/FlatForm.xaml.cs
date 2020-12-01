using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using RealtorObjects.Model;

namespace RealtorObjects.View
{
    /// <summary>
    /// Логика взаимодействия для FlatForm.xaml
    /// </summary>
    public partial class FlatForm : Window, INotifyPropertyChanged
    {
        public FlatForm() {
            InitializeComponent();
            DataContext = this;
        }
        int testInt = 0;
        double teest = 21.66123;
        CustomCommand increase;
        public CustomCommand Increase {
            get {
                return increase ??
                    (increase = new CustomCommand(obj => {
                        this.GetType().GetProperty((string)obj).SetValue(this, (double)this.GetType().GetProperty((string)obj).GetValue(this, null) + 0.05);
                    }));
            }
        }
        CustomCommand decrease;
        public CustomCommand Decrease {
            get {
                return decrease ??
                    (decrease = new CustomCommand(obj => {
                        this.GetType().GetProperty((string)obj).SetValue(this, (double)this.GetType().GetProperty((string)obj).GetValue(this, null) - 0.05);
                    }));
            }
        }

        public double Teest {
            get => teest;
            set {
                teest = value;
                OnPropertyChanged("Teest");
            }
        }

        public int TestInt {
            get => testInt;
            set {
                testInt = value;
                OnPropertyChanged();
            } 
        }

        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
        }


    }
}
