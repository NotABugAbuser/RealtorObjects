using Microsoft.Win32;
using MiscUtil;
using RealtorObjects.Model;
using RealtyModel.Events.Realty;
using RealtyModel.Events.UI;
using RealtyModel.Interface;
using RealtyModel.Model;
using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;
using RealtyModel.Model.RealtyObjects;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;

namespace RealtorObjects.ViewModel
{
    public class FlatFormViewModel : BaseViewModel, IDoubleNumericUpDown, IIntegerNumericUpDown
    {
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
        #region Fields
        private string title = "[Квартира] — Изменение";
        private Flat flat = new Flat();
        private CustomCommand cancel;
        private CustomCommand confirm;
        private CustomCommand changePrice;
        private CustomCommand removeImage;
        private CustomCommand addImages;
        private CustomCommand edit;
        private CustomCommand goLeft;
        private CustomCommand goRight;
        private CustomCommand showHideSlider;
        private readonly FlatOptions flatOptions = new FlatOptions();
        private LocationOptions locationOptions = new LocationOptions();
        private Visibility editBorderVisibility = Visibility.Visible;
        private Visibility sliderVisibility = Visibility.Collapsed;
        private byte[] currentImage;
        private int index = 0;
        #endregion
        public FlatFormViewModel() {

        }
        public FlatFormViewModel(bool isNew, string agentName) {
            if (isNew) {
                Title = "[Квартира] — Добавление";
                EditBorderVisibility = Visibility.Collapsed;
                Flat = Flat.CreateTestFlat();
                Flat.Agent = agentName;
            }
        }
        #region Slider
        public CustomCommand GoLeft => goLeft ?? (goLeft = new CustomCommand(obj => {
            index--;
            if (index == -1) {
                index = Flat.Album.PhotoCollection.Count - 1;
            }
            CurrentImage = Flat.Album.PhotoCollection[index];
        }));
        public CustomCommand GoRight => goRight ?? (goRight = new CustomCommand(obj => {
            index++;
            if (index == Flat.Album.PhotoCollection.Count) {
                index = 0;
            }
            CurrentImage = Flat.Album.PhotoCollection[index];
        }));
        public CustomCommand ShowHideSlider => showHideSlider ?? (showHideSlider = new CustomCommand(obj => {
            if (SliderVisibility == Visibility.Visible) {
                SliderVisibility = Visibility.Collapsed;
            } else {
                SliderVisibility = Visibility.Visible;
                index = 0;
            }
            CurrentImage = Flat.Album.PhotoCollection[index];
        }));
        #endregion
        public CustomCommand Edit => edit ?? (edit = new CustomCommand(obj => {
            EditBorderVisibility = Visibility.Collapsed;
        }));
        public CustomCommand AddImages => addImages ?? (addImages = new CustomCommand(obj => {
            OpenFileDialog openFileDialog = new OpenFileDialog() {
                Filter = "Файлы изображений (*.BMP; *.JPG; *.JPEG; *.PNG) | *.BMP; *.JPG; *.JPEG; *.PNG",
                Multiselect = true,
                Title = "Выбрать фотографии"
            };
            if (openFileDialog.ShowDialog() == true && openFileDialog.FileNames.Length != 0)
                foreach (string fileName in openFileDialog.FileNames) {
                    Byte[] data = File.ReadAllBytes(fileName);
                    Flat.Album.PhotoCollection.Add(data);
                }
        }));
        public CustomCommand RemoveImage => removeImage ?? (removeImage = new CustomCommand(obj => {
            Int32 number = Convert.ToInt32(obj);
            Flat.Album.PhotoCollection.RemoveAt(number);
        }));
        public CustomCommand ChangePrice => changePrice ?? (changePrice = new CustomCommand(obj => {
            var value = Convert.ToInt32(obj);
            Flat.Cost.Price += value;
        }));
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj => (obj as Window).Close()));

        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj => {
            if (new FieldChecking(Flat).CheckFieldsOfFlat()) {
                for (int i = 0; i < 100; i++) {
                    Client.AddFlat(Flat);

                }
            }
        }));
        public void ChangeProperty<T>(object obj, T step) {
            var objects = obj as object[];
            object instance = objects[0];
            string name = objects[1].ToString();
            PropertyInfo property = instance.GetType().GetProperty(name);
            T value = (T)property.GetValue(instance, null);
            property.SetValue(instance, Operator.Add(step, value));
        }
        #region Properties
        public string Title {
            get => title;
            set {
                title = value;
                OnPropertyChanged();
            }
        }
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
        public Visibility EditBorderVisibility {
            get => editBorderVisibility;
            set {
                editBorderVisibility = value;
                OnPropertyChanged();
            }
        }

        public Visibility SliderVisibility {
            get => sliderVisibility;
            set {
                sliderVisibility = value;
                OnPropertyChanged();
            }
        }

        public byte[] CurrentImage {
            get => currentImage;
            set {
                currentImage = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
