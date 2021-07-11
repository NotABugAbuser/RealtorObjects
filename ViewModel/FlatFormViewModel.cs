using BitmapImageDecoding;
using Microsoft.Win32;
using MiscUtil;
using RealtorObjects.Model;
using RealtorObjects.View;
using RealtyModel.Interface;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;
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
using System.Text.Json;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

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
        private Flat copiedFlat = new Flat();
        private Flat originalFlat = new Flat();
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
        private CustomCommand openGallery;
        private CustomCommand closeGallery;
        private CustomCommand showHideSlider;

        public CustomCommand OpenGallery => openGallery ?? (openGallery = new CustomCommand(obj => {
            int index = Convert.ToInt32(obj);
            Index = index;
            CurrentImage = Photos[index];
            SliderVisibility = Visibility.Visible;
        }));
        public CustomCommand CloseGallery => closeGallery ?? (closeGallery = new CustomCommand(obj => {
            SliderVisibility = Visibility.Collapsed;
            Index = 0;
        }));
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
        public Flat CopiedFlat {
            get => copiedFlat;
            set {
                copiedFlat = value;
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
        public FlatFormViewModel() {
        }
        public FlatFormViewModel(string agentName) {
            isNew = true;
            Title = $"[{agentName}: Квартира] — Добавление";
            currentAgent = agentName;
            CanEdit = true;
            if (Debugger.IsAttached) {
                CopiedFlat = Flat.CreateTestFlat();
            } else {
                CopiedFlat = new Flat();
            }
            CopiedFlat.Agent = agentName;
            CopiedFlat.RegistrationDate = DateTime.Now;
        }
        public FlatFormViewModel(Flat flat, string agentName) {
            CopiedFlat = flat.GetCopy();
            OriginalFlat = flat;
            Title = $"[{CopiedFlat.Agent}: Квартира #{CopiedFlat.Id}] — Просмотр";
            currentAgent = agentName;
        }

        public CustomCommand AddImages => addImages ?? (addImages = new CustomCommand(obj => {
            OpenFileDialog openFileDialog = new OpenFileDialog() {
                Filter = "Файлы изображений (*.BMP; *.JPG; *.JPEG; *.PNG) | *.BMP; *.JPG; *.JPEG; *.PNG",
                Multiselect = true,
                Title = "Выбрать фотографии"
            };
            Debug.WriteLine($"До добавления id {CopiedFlat.Album.Id}");
            bool hasFileNames = openFileDialog.ShowDialog() == true && openFileDialog.FileNames.Length != 0;
            if (hasFileNames) {
                foreach (String path in openFileDialog.FileNames) {
                    Photos.Add(BitmapImageDecoder.GetDecodedBytes(path, 30, 0));
                }
                CopiedFlat.Preview = BitmapImageDecoder.GetDecodedBytes(openFileDialog.FileNames[0], 0, 100);
                CopiedFlat.Album.PhotoCollection = BinarySerializer.Serialize(Photos);
                CurrentImage = Photos[0];
                Index = 0;
            }
        }));
        public CustomCommand RemoveImage => removeImage ?? (removeImage = new CustomCommand(obj => {
            Window window = obj as FlatFormV3;
            bool isSure = MessageBox.Show(window, "Удалить фотографию?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            if (isSure) {
                Photos.RemoveAt(Index);
                CopiedFlat.Album.PhotoCollection = BinarySerializer.Serialize(Photos);
                Index = 0;
                if (Photos.Count != 0) {
                    CurrentImage = Photos[0];
                } else {
                    SliderVisibility = Visibility.Collapsed;
                }
            }
        }));
        public CustomCommand AllowToEdit => allowToEdit ?? (allowToEdit = new CustomCommand(obj => {
            if (CheckAccess(CopiedFlat.Agent, currentAgent)) {
                CanEdit = true;
                Title = $"[{CopiedFlat.Agent}: Квартира #{CopiedFlat.Id}] — Редактирование";
            }
        }));
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj => {
            (obj as Window).Close();
        }));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj => {
            if (new FieldChecking(CopiedFlat).CheckFieldsOfFlat()) {
                if (isNew) {
                    Client.AddFlat(CopiedFlat);
                } else {
                    Client.UpdateFlat(CopiedFlat);
                    Debug.WriteLine($"Отправил с id {CopiedFlat.Album.Id}");
                }
                (obj as Window).Close();
            }
        }));
        private bool CheckAccess(string objectAgent, string currentAgent) {
            if (objectAgent == currentAgent) {
                return true;
            } else {
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
                        foreach (String path in openFileDialog.FileNames) {
                            images.Add(BitmapImageDecoder.GetDecodedBytes(path, 30, 0));
                        }
                        ((App)Application.Current).Dispatcher.Invoke(() => {
                            foreach (Byte[] image in images)
                                Photos.Add(image);
                        });
                        CopiedFlat.Preview = BitmapImageDecoder.GetDecodedBytes(openFileDialog.FileNames[0], 0, 100);
                        CopiedFlat.Album.PhotoCollection = BinarySerializer.Serialize(Photos);
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

        public Flat OriginalFlat {
            get => originalFlat; set => originalFlat = value;
        }
    }
}
