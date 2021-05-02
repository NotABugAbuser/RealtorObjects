﻿using Microsoft.Win32;
using MiscUtil;
using RandomFlatGenerator;
using RealtyModel.Event;
using RealtyModel.Interface;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

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
        private Flat flat;
        private string title;
        private bool isCurrentFlatNew = false;
        private CustomCommand cancel;
        private CustomCommand confirm;
        private CustomCommand changePrice;
        private CustomCommand removeImage;
        private CustomCommand addImages;
        private readonly FlatOptions flatOptions = new FlatOptions();
        private LocationOptions locationOptions = new LocationOptions();
        public FlatCreatedEventHandler FlatCreated;
        public FlatModifiedEventHandler FlatModified;
        public CustomCommand RemoveImage => removeImage ?? (removeImage = new CustomCommand(obj => {
            Test.RemoveAt(Convert.ToInt32(obj));
        }));
        public CustomCommand AddImages => addImages ?? (addImages = new CustomCommand(obj => {
            OpenFileDialog openFileDialog = new OpenFileDialog() { 
                Filter = "Файлы изображений (*.BMP; *.JPG; *.JPEG; *.PNG) | *.BMP; *.JPG; *.JPEG; *.PNG",
                Multiselect = true,
                Title = "Выбрать фотографии"
            };
            if (openFileDialog.ShowDialog() == true){
                foreach (string fileName in openFileDialog.FileNames){
                    Test.Add(File.ReadAllBytes(fileName));
                }
            }
        }));
        private ObservableCollection<byte[]> test = new ObservableCollection<byte[]>{ 
        };
        public Flat Flat
        {
            get => flat;
            set
            {
                flat = value;
                OnPropertyChanged();
            }
        }
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
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged();
            }
        }
        public bool IsCurrentFlatNew
        {
            get => isCurrentFlatNew;
            set => isCurrentFlatNew = value;
        }

        public CustomCommand ChangePrice => changePrice ?? (changePrice = new CustomCommand(obj =>
        {
            var value = Convert.ToInt32(obj);
            Flat.Cost.Price += value;
        }));
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj => (obj as Window).Close()));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj =>
        {
            Flat fla = new Flat()
            {
                Agent = "asdas",
                Album = new Album()
                {
                    Location = "фывфы",
                    PhotoList = new byte[100],
                    Preview = new byte[100],
                },
                Location = new Location()
                {
                    City = new City() { Name = "asd" },
                    District = new District() { Name = "asd" },
                    Street = new Street() { Name = "asd" },
                    HouseNumber = 15,
                    FlatNumber = 14,
                    HasBanner = true,
                    HasExchange = true
                },
                Cost = new Cost()
                {
                    Area = 10,
                    HasMortgage = true,
                    HasPercents = true,
                    HasVAT = true,
                    Price = 1000
                },
                Customer = new Customer()
                {
                    Name = "выав",
                    PhoneNumbers = "12312321"
                },
                GeneralInfo = new BaseInfo()
                {
                    Ceiling = 10,
                    Condition = "asd",
                    Convenience = "asd",
                    Description = "asd",
                    General = 10,
                    Heating = "asd",
                    Kitchen = 10,
                    Living = 10,
                    RoomCount = 10,
                    Water = "asd",
                    Year = 1950
                },
                Info = new FlatInfo()
                {
                    Balcony = "asd",
                    Bath = "asd",
                    Bathroom = "asd",
                    Floor = "asd",
                    Fund = "asd",
                    HasChute = true,
                    HasElevator = true,
                    HasGarage = true,
                    HasImprovedLayout = true,
                    HasRenovation = true,
                    IsCorner = true,
                    IsPrivatised = true,
                    IsSeparated = true,
                    Kvl = 10,
                    Loggia = "asd",
                    Material = "asd",
                    Rooms = "asd",
                    Type = "asd",
                    TypeOfRooms = "asd",
                    Windows = "asd"
                },
                HasExclusive = true,
                IsSold = true,
                LastUpdateTime = DateTime.Now,
                ObjectType = "asd",
                Status = Status.Active
            };
            //for (Int32 i = 0; i < 10; i++)
            //{
            //    FlatCreated?.Invoke(this, new FlatCreatedEventArgs(fla));
            //    fla.Location.FlatNumber++;
            //    Thread.Sleep(500);
            //}
            FlatCreated?.Invoke(this, new FlatCreatedEventArgs(Flat));
        }));

        public ObservableCollection<byte[]> Test { get => test; set => test = value; }

        public void ChangeProperty<T>(object obj, T step)
        {
            var objects = obj as object[];
            object instance = objects[0];
            string name = objects[1].ToString();
            PropertyInfo property = instance.GetType().GetProperty(name);
            T value = (T)property.GetValue(instance, null);
            property.SetValue(instance, Operator.Add(step, value));
        }

        public FlatFormViewModel()
        {
        }
    }
}
