using MiscUtil;
using RealtyModel.Interface;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RealtorObjects.ViewModel
{
    public class FlatFormViewModel : BaseViewModel, IDoubleNumericUpDown, IIntegerNumericUpDown
    {
        private string testString = "руддщ";
        private int testInt = 20;
        private double testDouble = 21.66123;
        private CustomCommand testCommand;
        private CustomCommand increaseDouble;
        private CustomCommand decreaseDouble;
        private CustomCommand increaseInteger;
        private CustomCommand decreaseInteger;
        private CustomCommand cancel;
        private CustomCommand confirm;
        private CustomCommand changePrice;
        private Flat flat = new Flat();
        readonly FlatOptions flatOptions = new FlatOptions();
        #region UpDownOperations
        public CustomCommand IncreaseDouble => increaseDouble ??
            (increaseDouble = new CustomCommand(obj => {
                ChangeProperty<double>(obj, 0.05);
            }));
        public CustomCommand IncreaseInteger => increaseInteger ??
            (increaseInteger = new CustomCommand(obj => {
                ChangeProperty<int>(obj, 1);
            }));
        public CustomCommand DecreaseDouble => decreaseDouble ??
            (decreaseDouble = new CustomCommand(obj => {
                ChangeProperty<double>(obj, -0.05);
            }));
        public CustomCommand DecreaseInteger => decreaseInteger ??
            (decreaseInteger = new CustomCommand(obj => {
                ChangeProperty<int>(obj, -1);
            }));
        #endregion
        #region TestMethods
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
        public CustomCommand TestCommand => testCommand ??
            (testCommand = new CustomCommand(obj => {
                MessageBox.Show(JsonSerializer.Serialize(Flat).Replace(',', '\n'));
            }));
        #endregion

        public FlatFormViewModel() {

        }
        public FlatFormViewModel(Flat flat) {
            this.Flat = flat;
        }
        public CustomCommand ChangePrice => changePrice ?? (changePrice = new CustomCommand(obj => {
            var value = Convert.ToInt32(obj);
            Flat.Cost.Price += value;
        }));
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj => (obj as Window).Close()));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj => { 
        
        }));
        
        public Flat Flat {
            get => flat;
            set {
                flat = value;
                OnPropertyChanged();
            }
        }
        public FlatOptions FlatOptions {
            get => flatOptions;
        }
        public void ChangeProperty<T>(object obj, T step) {
            var objects = obj as object[];
            string name = objects[1].ToString();
            object instance = objects[0];

            PropertyInfo property = instance.GetType().GetProperty(name);
            T value = (T)property.GetValue(instance, null);
            property.SetValue(instance, Operator.Add(step, value));
        }
        
    }
}
