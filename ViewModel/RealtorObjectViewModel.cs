using BitmapImageDecoding;
using Microsoft.Win32;
using RealtorObjects.Model;
using RealtyModel.Model;
using RealtyModel.Model.Operations;
using RealtyModel.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    public class RealtorObjectFormViewModel : BaseViewModel
    {
        protected string title;
        protected CustomCommand cancel;
        protected CustomCommand allowToEdit;
        protected CustomCommand addImages;
        protected CustomCommand removeImage;
        protected CustomCommand goLeft;
        protected CustomCommand goRight;
        protected CustomCommand openGallery;
        protected CustomCommand closeGallery;
        protected byte[] currentImage;
        protected string currentAgent = "";
        protected int index = 0;
        protected bool isNew = false;
        protected ObservableCollection<Street> streets = new ObservableCollection<Street>();
        protected ObservableCollection<byte[]> photos = new ObservableCollection<byte[]>();
        protected CustomCommand confirm;
        readonly protected ComboBoxOptions comboBoxOptions = new ComboBoxOptions();
        protected bool canEdit = false;
        protected Visibility sliderVisibility = Visibility.Collapsed;
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj => {
            (obj as Window).Close();
        }));
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
                CurrentImage = Photos[0];
                Index = 0;
            }
        }));
        #region Gallery
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
        public CustomCommand RemoveImage => removeImage ?? (removeImage = new CustomCommand(obj => {
            Window window = obj as Window;
            bool isSure = MessageBox.Show(window, "Удалить фотографию?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            if (isSure) {
                Photos.RemoveAt(Index);
                Index = 0;
                if (Photos.Count != 0) {
                    CurrentImage = Photos[0];
                } else {
                    SliderVisibility = Visibility.Collapsed;
                }
            }
        }));
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
        protected bool CheckAccess(string objectAgent, string currentAgent) {
            if (objectAgent == currentAgent) {
                return true;
            } else {
                OperationNotification.Notify(ErrorCode.WrongAgent);
                return false;
            }
        }
        public string Title {
            get => title;
            set {
                title = value;
                OnPropertyChanged();
            }
        }
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
