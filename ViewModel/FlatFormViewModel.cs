using BitmapImageDecoding;
using Microsoft.Win32;
using MiscUtil;
using RealtorObjects.Model;
using RealtorObjects.View;
using RealtyModel.Interface;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;
using RealtyModel.Model.RealtyObjects;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;

namespace RealtorObjects.ViewModel
{
    public class FlatFormViewModel : BaseViewModel
    {
        #region Fields
        private int index = 0;
        private bool isNew = false;
        private byte[] currentImage;
        private string title = "[Квартира] — Изменение";
        private string currentAgent = "";

        private ObservableCollection<Street> streets = new ObservableCollection<Street>();
        private Flat flat = new Flat() { Location = new Location() { Street = new Street() } };
        private bool canEdit = false;
        readonly private ComboBoxOptions comboBoxOptions = new ComboBoxOptions();
        private Visibility sliderVisibility = Visibility.Collapsed;
        private ObservableCollection<byte[]> photos = new ObservableCollection<byte[]>();
        private CustomCommand cancel;
        private CustomCommand confirm;
        private CustomCommand removeImage;
        private CustomCommand addImages;
        private CustomCommand allowToEdit;
        private CustomCommand goLeft;
        private CustomCommand goRight;
        private CustomCommand showHideSlider;
        private string streetName;
        #endregion
        #region Properties
        public int Index {
            get => index;
            set {
                index = value;
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
        public ObservableCollection<Street> Streets {
            get => streets;
            set {
                streets = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<byte[]> Photos {
            get => photos;
            set {
                photos = value;
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
        #endregion

        public String StreetName {
            get => streetName;
            set {
                streetName = value;
                OnPropertyChanged();
            }
        }

        public FlatFormViewModel() {
        }
        public FlatFormViewModel(string agentName) {
            isNew = true;
            Title = $"[{agentName}: Квартира] — Добавление";
            currentAgent = agentName;
            if (Debugger.IsAttached)
                Flat = Flat.CreateTestFlat();
            else Flat = Flat.GetEmptyInstance();
            Flat.Agent = agentName;
            Flat.RegistrationDate = DateTime.Now;
            Flat.Album.PhotoCollection = Array.Empty<byte>();
            StreetName = Flat.Location.Street.Name;
        }
        public FlatFormViewModel(Flat flat, string agentName) {
            Flat = flat;

            Title = $"[{Flat.Agent}: Квартира #{Flat.Id}] — Просмотр";
            currentAgent = agentName;
            StreetName = Flat.Location.Street.Name;
        }

        public CustomCommand AddImages => addImages ?? (addImages = new CustomCommand(obj => {
            OpenFileDialog openFileDialog = new OpenFileDialog() {
                Filter = "Файлы изображений (*.BMP; *.JPG; *.JPEG; *.PNG) | *.BMP; *.JPG; *.JPEG; *.PNG",
                Multiselect = true,
                Title = "Выбрать фотографии"
            };
            bool hasFileNames = openFileDialog.ShowDialog() == true && openFileDialog.FileNames.Length != 0;
            if (hasFileNames) {
                foreach (String path in openFileDialog.FileNames) {
                    Photos.Add(BitmapImageDecoder.GetDecodedBytes(path, 30, 0));
                }
                Flat.Preview = BitmapImageDecoder.GetDecodedBytes(openFileDialog.FileNames[0], 0, 100);
                Flat.Album.PhotoCollection = BinarySerializer.Serialize(Photos);
                CurrentImage = Photos[0];
                Index = 0;
            }
        }));
        public CustomCommand AllowToEdit => allowToEdit ?? (allowToEdit = new CustomCommand(obj => {
            CanEdit = true;
            //if (CheckAccess(Flat.Agent, currentAgent)) {
            //    //EditBorderVisibility = Visibility.Collapsed;
            //    Title = $"[{Flat.Agent}: Квартира #{Flat.Id}] — Редактирование";
            //}
        }));
        public CustomCommand RemoveImage => removeImage ?? (removeImage = new CustomCommand(obj => {
            Window window = obj as FlatFormV2;
            bool isSure = MessageBox.Show(window, "Удалить фотографию?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            if (isSure) {
                Photos.RemoveAt(Index);
                Flat.Album.PhotoCollection = BinarySerializer.Serialize(Photos);
                Index = 0;
                if (Photos.Count != 0) {
                    CurrentImage = Photos[0];
                } else {
                    SliderVisibility = Visibility.Collapsed;
                }
            }
        }));
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj => {
            (obj as Window).Close();
        }
        ));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj => {
            try {
                if (Flat.GeneralInfo.CurrentLevel <= Flat.GeneralInfo.LevelCount) {
                    if (Flat.Location.Street == null)
                        Flat.Location.Street = new Street() { Id = 0, Name = StreetName };
                    if (new FieldChecking(Flat).CheckFieldsOfFlat()) {
                        if (isNew)
                            Client.AddFlat(Flat);
                        else
                            Client.UpdateFlat(Flat);
                        (obj as Window).Close();
                    }
                } else OperationNotification.WarningNotify(ErrorCode.WrongData, "Этажи введены неверно");
            } catch (FormatException) {
                OperationNotification.Notify(ErrorCode.WrongFormat);
            }
        }));
        private bool CheckAccess(string objectAgent, string currentAgent) {
            if (objectAgent == currentAgent)
                return true;
            else {
                OperationNotification.Notify(ErrorCode.WrongAgent);
                return false;
            }
        }
        #region Slider
        public CustomCommand GoLeft => goLeft ?? (goLeft = new CustomCommand(obj => {
            Index--;
            if (Index == -1) {
                Index = Photos.Count - 1;
            }
            CurrentImage = Photos[Index];
        }));
        public CustomCommand GoRight => goRight ?? (goRight = new CustomCommand(obj => {
            Index++;
            if (Index == Photos.Count) {
                Index = 0;
            }
            CurrentImage = Photos[Index];
        }));
        public CustomCommand ShowHideSlider => showHideSlider ?? (showHideSlider = new CustomCommand(obj => {
            if (SliderVisibility == Visibility.Visible) {
                SliderVisibility = Visibility.Collapsed;
            } else {
                SliderVisibility = Visibility.Visible;
                Index = 0;
            }
        }));
        #endregion

        public AsyncCommand AddImagesAsync {
            get => new AsyncCommand(() => {
                return Task.Run(() => {
                    OpenFileDialog openFileDialog = new OpenFileDialog() {
                        Filter = "Файлы изображений (*.BMP; *.JPG; *.JPEG; *.PNG) | *.BMP; *.JPG; *.JPEG; *.PNG",
                        Multiselect = true,
                        Title = "Выбрать фотографии"
                    };
                    bool hasFileNames = openFileDialog.ShowDialog() == true && openFileDialog.FileNames.Length != 0;
                    if (hasFileNames) {
                        List<Byte[]> images = new List<Byte[]>();
                        foreach (String path in openFileDialog.FileNames)
                            images.Add(BitmapImageDecoder.GetDecodedBytes(path, 30, 0));
                        //Parallel.ForEach(openFileDialog.FileNames, new Action<String>((path) =>
                        //{
                        //    images.Add(BitmapImageDecoder.GetDecodedBytes(path, 30, 0));
                        //}));

                        ((App)Application.Current).Dispatcher.Invoke(() => {
                            foreach (Byte[] image in images)
                                Photos.Add(image);
                        });
                        Flat.Preview = BitmapImageDecoder.GetDecodedBytes(openFileDialog.FileNames[0], 0, 100);
                        Flat.Album.PhotoCollection = BinarySerializer.Serialize(Photos);
                        CurrentImage = images[0];
                        Index = 0;
                        //images.Clear();
                        GC.Collect();
                        GC.SuppressFinalize(images);
                        GC.Collect();

                    }
                });
            });
        }

        public ComboBoxOptions ComboBoxOptions => comboBoxOptions;

        public bool CanEdit {
            get => canEdit;
            set {
                canEdit = value;
                OnPropertyChanged();
            }
        }
    }
}
