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

namespace RealtorObjects.ViewModel
{
    public class FlatFormViewModel : BaseViewModel, IIntegerNumericUpDown
    {
        #region Fields
        private int index = 0;
        private bool isNew = false;
        private byte[] currentImage;
        private string title = "[Квартира] — Изменение";
        private string currentAgent = "";
        private readonly FlatOptions flatOptions = new FlatOptions();
        private ObservableCollection<Street> streets = new ObservableCollection<Street>();
        private District[] districts = new District[]
        {
            District.Авиагородок,
            District.Авиаторов_сквер,
            District.Азовский,
            District.Ветлечебница,
            District.ВЖМ,
            District.Гайдара,
            District.Залесье,
            District.Заря,
            District.ЗЖМ,
            District.Красный_сад,
            District.Наливная,
            District.ПЧЛ,
            District.РДВС,
            District.СЖМ,
            District.Солёное_озеро,
            District.Солнечный,
            District.Центр
        };
        private City[] cities = new City[]
        {
            City.Азов,
            City.Батайск,
            City.Ростов_на_Дону
        };
        private Flat flat = new Flat();
        private Visibility editBorderVisibility = Visibility.Visible;
        private Visibility sliderVisibility = Visibility.Collapsed;
        private ObservableCollection<byte[]> photos = new ObservableCollection<byte[]>();
        private CustomCommand cancel;
        private CustomCommand confirm;
        private CustomCommand changePrice;
        private CustomCommand removeImage;
        private CustomCommand addImages;
        private CustomCommand edit;
        private CustomCommand goLeft;
        private CustomCommand goRight;
        private CustomCommand showHideSlider;
        #endregion
        #region Properties
        public int Index
        {
            get => index;
            set
            {
                index = value;
                OnPropertyChanged();
            }
        }
        public byte[] CurrentImage
        {
            get => currentImage;
            set
            {
                currentImage = value;
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
        public Flat Flat
        {
            get => flat;
            set
            {
                flat = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Street> Streets
        {
            get => streets;
            set
            {
                streets = value;
                OnPropertyChanged();
            }
        }
        public District[] Districts
        {
            get => districts;
            set => districts = value;
        }
        public City[] Cities
        {
            get => cities;
            set => cities = value;
        }
        public FlatOptions FlatOptions
        {
            get => flatOptions;
        }
        public ObservableCollection<byte[]> Photos
        {
            get => photos;
            set
            {
                photos = value;
                OnPropertyChanged();
            }
        }
        public Visibility SliderVisibility
        {
            get => sliderVisibility;
            set
            {
                sliderVisibility = value;
                OnPropertyChanged();
            }
        }
        public Visibility EditBorderVisibility
        {
            get => editBorderVisibility;
            set
            {
                editBorderVisibility = value;
                OnPropertyChanged();
            }
        }
        #endregion
        private string kitchenToParse = "";
        private string livingToParse = "";
        private string generalToParse = "";
        private string ceilingToParse = "";
        private Int32 streetId = 0;
        private String street = "";
        private CustomCommand changeStreet;
        public String Street
        {
            get => street;
            set
            {
                street = value;
                OnPropertyChanged();
            }
        }
        public CustomCommand ChangeStreet
        {
            get => changeStreet ?? (changeStreet = new CustomCommand(obj =>
            {
                if (Flat.Location.Street != null)
                {
                    streetId = Flat.Location.Street.Id;
                    Street = Flat.Location.Street.Name;
                }
            }));
        }

        public FlatFormViewModel()
        {
        }
        public FlatFormViewModel(string agentName)
        {
            currentAgent = agentName;

            Title = $"[{agentName}: Квартира] — Добавление";
            isNew = true;
            EditBorderVisibility = Visibility.Collapsed;

            if (Debugger.IsAttached)
                Flat = Flat.CreateTestFlat();
            else Flat = Flat.GetEmptyInstance();
            Flat.Agent = agentName;
            Flat.RegistrationDate = DateTime.Now;
            Flat.Album.PhotoCollection = Array.Empty<byte>();
            Street = Flat.Location.Street.Name;
        }
        public FlatFormViewModel(Flat flat, string agentName)
        {
            Title = $"[{Flat.Agent}: Квартира #{Flat.Id}] — Просмотр";
            Flat = flat;
            currentAgent = agentName;
            Street = Flat.Location.Street.Name;
        }

        public CustomCommand AddImages => addImages ?? (addImages = new CustomCommand(obj =>
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Файлы изображений (*.BMP; *.JPG; *.JPEG; *.PNG) | *.BMP; *.JPG; *.JPEG; *.PNG",
                Multiselect = true,
                Title = "Выбрать фотографии"
            };
            bool hasFileNames = openFileDialog.ShowDialog() == true && openFileDialog.FileNames.Length != 0;
            if (hasFileNames)
            {
                foreach (String path in openFileDialog.FileNames)
                    Photos.Add(BitmapImageDecoder.GetDecodedBytes(path, 30, 0));
                Flat.Preview = BitmapImageDecoder.GetDecodedBytes(openFileDialog.FileNames[0], 0, 100);
                Flat.Album.PhotoCollection = BinarySerializer.Serialize(Photos);
                CurrentImage = Photos[0];
                Index = 0;
            }
        }));
        public CustomCommand Edit => edit ?? (edit = new CustomCommand(obj =>
        {
            if (CheckAccess(Flat.Agent, currentAgent))
            {
                EditBorderVisibility = Visibility.Collapsed;
                Title = $"[{Flat.Agent}: Квартира #{Flat.Id}] — Редактирование";
            }
        }));
        public CustomCommand RemoveImage => removeImage ?? (removeImage = new CustomCommand(obj =>
        {
            Window window = obj as FlatFormV2;
            bool isSure = MessageBox.Show(window, "Удалить фотографию?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            if (isSure)
            {
                Photos.RemoveAt(Index);
                Flat.Album.PhotoCollection = BinarySerializer.Serialize(Photos);
                Index = 0;
                if (Photos.Count != 0)
                {
                    CurrentImage = Photos[0];
                }
                else
                {
                    SliderVisibility = Visibility.Collapsed;
                }
            }
        }));
        public CustomCommand ChangePrice => changePrice ?? (changePrice = new CustomCommand(obj =>
        {
            var value = Convert.ToInt32(obj);
            Flat.Cost.Price += value;
        }));
        public CustomCommand Cancel => cancel ?? (cancel = new CustomCommand(obj =>
        {
            (obj as Window).Close();
        }
        ));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj =>
        {
            try
            {
                Flat.GeneralInfo.Ceiling = float.Parse(CeilingToParse, CultureInfo.InvariantCulture);
                Flat.GeneralInfo.General = float.Parse(GeneralToParse, CultureInfo.InvariantCulture);
                Flat.GeneralInfo.Living = float.Parse(LivingToParse, CultureInfo.InvariantCulture);
                Flat.GeneralInfo.Kitchen = float.Parse(KitchenToParse, CultureInfo.InvariantCulture);
                if (new FieldChecking(Flat).CheckFieldsOfFlat())
                {
                    if (isNew)
                        Client.AddFlat(Flat);
                    else
                        Client.UpdateFlat(Flat);
                    (obj as Window).Close();
                }
            }
            catch (FormatException)
            {
                OperationNotification.Notify(ErrorCode.WrongFormat);
            }

        }));
        private bool CheckAccess(string objectAgent, string currentAgent)
        {
            if (objectAgent == currentAgent)
                return true;
            else
            {
                OperationNotification.Notify(ErrorCode.WrongAgent);
                return false;
            }
        }
        public void ChangeProperty<T>(object obj, T step)
        {
            var objects = obj as object[];
            object instance = objects[0];
            string name = objects[1].ToString();
            PropertyInfo property = instance.GetType().GetProperty(name);
            T value = (T)property.GetValue(instance, null);
            property.SetValue(instance, Operator.Add(step, value));
        }
        #region Slider
        public CustomCommand GoLeft => goLeft ?? (goLeft = new CustomCommand(obj =>
        {
            Index--;
            if (Index == -1)
            {
                Index = Photos.Count - 1;
            }
            CurrentImage = Photos[Index];
        }));
        public CustomCommand GoRight => goRight ?? (goRight = new CustomCommand(obj =>
        {
            Index++;
            if (Index == Photos.Count)
            {
                Index = 0;
            }
            CurrentImage = Photos[Index];
        }));
        public CustomCommand ShowHideSlider => showHideSlider ?? (showHideSlider = new CustomCommand(obj =>
        {
            if (SliderVisibility == Visibility.Visible)
            {
                SliderVisibility = Visibility.Collapsed;
            }
            else
            {
                SliderVisibility = Visibility.Visible;
                Index = 0;
            }
        }));
        #endregion
        #region UpDownOperations
        private CustomCommand increaseInteger;
        private CustomCommand decreaseInteger;

        public CustomCommand IncreaseInteger => increaseInteger ??
            (increaseInteger = new CustomCommand(obj =>
            {
                ChangeProperty<int>(obj, 1);
            }));
        public CustomCommand DecreaseInteger => decreaseInteger ??
            (decreaseInteger = new CustomCommand(obj =>
            {
                ChangeProperty<int>(obj, -1);
            }));
        #endregion

        public AsyncCommand AddImagesAsync
        {
            get => new AsyncCommand(() =>
            {
                return Task.Run(() =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        Filter = "Файлы изображений (*.BMP; *.JPG; *.JPEG; *.PNG) | *.BMP; *.JPG; *.JPEG; *.PNG",
                        Multiselect = true,
                        Title = "Выбрать фотографии"
                    };
                    bool hasFileNames = openFileDialog.ShowDialog() == true && openFileDialog.FileNames.Length != 0;
                    if (hasFileNames)
                    {
                        List<Byte[]> images = new List<Byte[]>();
                        foreach (String path in openFileDialog.FileNames)
                            images.Add(BitmapImageDecoder.GetDecodedBytes(path, 30, 0));
                        //Parallel.ForEach(openFileDialog.FileNames, new Action<String>((path) =>
                        //{
                        //    images.Add(BitmapImageDecoder.GetDecodedBytes(path, 30, 0));
                        //}));

                        ((App)Application.Current).Dispatcher.Invoke(() =>
                        {
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
        public string KitchenToParse
        {
            get => Flat.GeneralInfo.Kitchen.ToString();
            set => kitchenToParse = value;
        }
        public string LivingToParse
        {
            get => Flat.GeneralInfo.Living.ToString();
            set => livingToParse = value;
        }
        public string GeneralToParse
        {
            get => Flat.GeneralInfo.General.ToString();
            set => generalToParse = value;
        }
        public string CeilingToParse
        {
            get => Flat.GeneralInfo.Ceiling.ToString();
            set => ceilingToParse = value;
        }
    }
}
