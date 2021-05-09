﻿using Microsoft.Win32;
using MiscUtil;
using RealtorObjects.Model;
using RealtyModel.Events.Realty;
using RealtyModel.Events.UI;
using RealtyModel.Interface;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using RealtyModel.Service;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    public class FlatFormViewModel : BaseViewModel, IDoubleNumericUpDown, IIntegerNumericUpDown
    {
        #region TestProperties
        private CustomCommand testCommand;
        private string testString = "руддщ";
        private int testInt = 20;
        private double testDouble = 21.66123;
        public double TestDouble
        {
            get => testDouble;
            set
            {
                testDouble = value;
                OnPropertyChanged();
            }
        }
        public int TestInt
        {
            get => testInt;
            set
            {
                testInt = value;
                OnPropertyChanged();
            }
        }
        public string TestString
        {
            get => testString;
            set
            {
                testString = value;
                OnPropertyChanged();
            }
        }
        public CustomCommand TestCommand => testCommand ??
            (testCommand = new CustomCommand(obj =>
            {
                MessageBox.Show(JsonSerializer.Serialize(Flat).Replace(',', '\n'));
            }));
        #endregion
        #region UpDownOperations
        private CustomCommand increaseDouble;
        private CustomCommand decreaseDouble;
        private CustomCommand increaseInteger;
        private CustomCommand decreaseInteger;
        public CustomCommand IncreaseDouble => increaseDouble ??
            (increaseDouble = new CustomCommand(obj =>
            {
                ChangeProperty<Single>(obj, 0.05f);
            }));
        public CustomCommand IncreaseInteger => increaseInteger ??
            (increaseInteger = new CustomCommand(obj =>
            {
                ChangeProperty<int>(obj, 1);
            }));
        public CustomCommand DecreaseDouble => decreaseDouble ??
            (decreaseDouble = new CustomCommand(obj =>
            {
                ChangeProperty<Single>(obj, -0.05f);
            }));
        public CustomCommand DecreaseInteger => decreaseInteger ??
            (decreaseInteger = new CustomCommand(obj =>
            {
                ChangeProperty<int>(obj, -1);
            }));
        #endregion
        #region Fields
        private string title;
        private bool isCurrentFlatNew = false;
        private Flat flat;
        private CustomCommand cancel;
        private CustomCommand confirm;
        private CustomCommand changePrice;
        private CustomCommand removeImage;
        private CustomCommand addImages;
        private readonly FlatOptions flatOptions = new FlatOptions();
        private LocationOptions locationOptions = new LocationOptions();

        public FlatCreatingEventHandler FlatCreating;
        public FlatChangingEventHandler FlatChanging;
        private ObservableCollection<byte[]> test = new ObservableCollection<byte[]> { };
        #endregion

        public bool IsCurrentFlatNew
        {
            get => isCurrentFlatNew;
            set => isCurrentFlatNew = value;
        }
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }
        public Flat Flat
        {
            get => flat;
            set
            {
                flat = value;
                OnPropertyChanged();
            }
        }
        public Location CurrentLocation { get; set; }
        public FlatOptions FlatOptions
        {
            get => flatOptions;
        }
        public LocationOptions LocationOptions
        {
            get => locationOptions;
            set
            {
                locationOptions = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<byte[]> Test { get => test; set => test = value; }

        public FlatFormViewModel()
        {
        }

        public CustomCommand AddImagesTest => addImages ?? (addImages = new CustomCommand(obj =>
        {
            Flat.Album.PhotoCollection = new ObservableCollection<byte[]>();
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Файлы изображений (*.BMP; *.JPG; *.JPEG; *.PNG) | *.BMP; *.JPG; *.JPEG; *.PNG",
                Multiselect = true,
                Title = "Выбрать фотографии"
            };
            if (openFileDialog.ShowDialog() == true)
                foreach (string fileName in openFileDialog.FileNames)
                {
                    Byte[] data = File.ReadAllBytes(fileName);
                    Flat.Album.PhotoCollection.Add(data);
                    Test.Add(data);
                }
            Flat.Album.WriteLocation(Flat.Location);
            if (Flat.Album.PhotoCollection.Count > 0)
                Flat.Album.PreviewJson = JsonSerializer.Serialize(Flat.Album.PhotoCollection[0]);
            else Flat.Album.PreviewJson = "";
        }));
        public CustomCommand RemoveImage => removeImage ?? (removeImage = new CustomCommand(obj =>
        {
            Int32 number = Convert.ToInt32(obj);
            Test.RemoveAt(number);
            Flat.Album.PhotoCollection.RemoveAt(number);
        }));
        public CustomCommand ChangePrice => changePrice ?? (changePrice = new CustomCommand(obj =>
        {
            var value = Convert.ToInt32(obj);
            Flat.Cost.Price += value;
        }));
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj => (obj as Window).Close()));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj =>
        {
            CurrentLocation = Flat.Location.GetCopy();
            if (new FieldChecking(Flat).CheckFieldsOfFlat())
            {
                FlatCreating?.Invoke(this, new FlatCreatingEventArgs(Flat));
                (obj as Window).Close();
            }
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
    }
}
