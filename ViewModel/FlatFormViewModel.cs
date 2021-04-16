using MiscUtil;
using RealtyModel.Event;
using RealtyModel.Interface;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RealtorObjects.ViewModel
{
    public class FlatFormViewModel : BaseViewModel, IDoubleNumericUpDown, IIntegerNumericUpDown
    {
        public CustomCommand ChangePrice => changePrice ?? (changePrice = new CustomCommand(obj => {
            var value = Convert.ToInt32(obj);
            Flat.Cost.Price += value;
        }));
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj => (obj as Window).Close()));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj => {
            FlatCreated?.Invoke(this, new FlatCreatedEventArgs(this.Flat));
        }));
        public void ChangeProperty<T>(object obj, T step)
        {
            var objects = obj as object[];
            object instance = objects[0];
            string name = objects[1].ToString();
            PropertyInfo property = instance.GetType().GetProperty(name);
            T value = (T)property.GetValue(instance, null);
            property.SetValue(instance, Operator.Add(step, value));
        }

        private Flat flat;
        private string title;
        private bool isCurrentFlatNew = false;
        private CustomCommand cancel;
        private CustomCommand confirm;
        private CustomCommand changePrice;
        private readonly FlatOptions flatOptions = new FlatOptions();
        private LocationOptions locationOptions = new LocationOptions();
        public FlatCreatedEventHandler FlatCreated;
        public FlatModifiedEventHandler FlatModified;

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
        public LocationOptions LocationOptions {
            get => locationOptions;
            set {
                locationOptions = value;
                OnPropertyChanged();
            }
        }
        public string Title {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }
        public bool IsCurrentFlatNew {
            get => isCurrentFlatNew;
            set => isCurrentFlatNew = value;
        }
        #region UpDownOperations
        private CustomCommand increaseDouble;
        private CustomCommand decreaseDouble;
        private CustomCommand increaseInteger;
        private CustomCommand decreaseInteger;
        public CustomCommand IncreaseDouble => increaseDouble ??
            (increaseDouble = new CustomCommand(obj => {
                ChangeProperty<Single>(obj, 0.05f);
            }));
        public CustomCommand IncreaseInteger => increaseInteger ??
            (increaseInteger = new CustomCommand(obj => {
                ChangeProperty<int>(obj, 1);
            }));
        public CustomCommand DecreaseDouble => decreaseDouble ??
            (decreaseDouble = new CustomCommand(obj => {
                ChangeProperty<Single>(obj, -0.05f);
            }));
        public CustomCommand DecreaseInteger => decreaseInteger ??
            (decreaseInteger = new CustomCommand(obj => {
                ChangeProperty<int>(obj, -1);
            }));
        #endregion
        #region TestProperties
        private CustomCommand testCommand;
        private string testString = "руддщ";
        private int testInt = 20;
        private double testDouble = 21.66123;
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
    }
}
