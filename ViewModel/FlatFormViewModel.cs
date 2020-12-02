using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.ViewModel
{
    class FlatFormViewModel : INotifyPropertyChanged, IDoubleNumericUpDown, IIntegerNumericUpDown
    {
        string testString = "руддщ";
        int testInt = 20;
        double testDouble = 21.66123;
        CustomCommand increaseDouble;
        CustomCommand decreaseDouble;
        CustomCommand increaseInteger;
        CustomCommand decreaseInteger;

        
        public CustomCommand IncreaseDouble => increaseDouble ??
            (increaseDouble = new CustomCommand(obj => {
                ChangeProperty((string)obj, 0.05);
            }));
        public CustomCommand IncreaseInteger => increaseInteger ??
            (increaseInteger = new CustomCommand(obj => {
                ChangeProperty((string)obj, 1);
            }));
        public CustomCommand DecreaseDouble => decreaseDouble ??
            (decreaseDouble = new CustomCommand(obj => {
                ChangeProperty((string)obj, -0.05);
            }));
        public CustomCommand DecreaseInteger => decreaseInteger ??
            (decreaseInteger = new CustomCommand(obj => {
                ChangeProperty((string)obj, -1);
            }));

        public double TestDouble {
            get => testDouble;
            set {
                testDouble = value;
                OnPropertyChanged();
            }
        }
        public int TestInt {
            get => testInt;
            set {
                testInt = value;
                OnPropertyChanged();
            }
        }
        public string TestString {
            get => testString;
            set {
                testString = value;
                OnPropertyChanged();
            }
        }
        public void ChangeProperty(string name, double value) {
            this.GetType().GetProperty(name).SetValue(this, (double)this.GetType().GetProperty(name).GetValue(this, null) + value);
        }
        public void ChangeProperty(string name, sbyte value) {
            this.GetType().GetProperty(name).SetValue(this, (int)(this.GetType().GetProperty(name).GetValue(this, null)) + value);
        }
        public void OnPropertyChanged([CallerMemberName] string property = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
