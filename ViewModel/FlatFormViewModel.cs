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
    public class FlatFormViewModel : RealtorObjectFormViewModel
    {
        private Flat copiedFlat = new Flat();
        private Flat originalFlat = new Flat();
        public FlatFormViewModel() {
        }
        public FlatFormViewModel(string agentName) {
            isNew = true;
            Title = $"[Квартирный объект]  —  Добавление";
            currentAgent = agentName;
            CanEdit = true;
            if (Debugger.IsAttached) {
                CopiedFlat = Flat.CreateTestFlat();
            } else {
                CopiedFlat = new Flat();
            }
            CopiedFlat.Agent = agentName;
        }
        public FlatFormViewModel(Flat flat, string agentName) {
            CopiedFlat = flat.GetCopy();
            OriginalFlat = flat;
            Title = $"[#{CopiedFlat.Id}] [Тип: {CopiedFlat.GeneralInfo.ObjectType}] [Создатель заявки: {CopiedFlat.Agent}]  —  Просмотр";
            currentAgent = agentName;
        }
        public CustomCommand AllowToEdit => allowToEdit ?? (allowToEdit = new CustomCommand(obj => {
            if (CheckAccess(CopiedFlat.Agent, currentAgent)) {
                CanEdit = true;
                Title = $"[#{CopiedFlat.Id}] [Тип: {CopiedFlat.GeneralInfo.ObjectType}] [Создатель заявки: {CopiedFlat.Agent}]  —  Редактирование";
            }
        }));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj => {
            if (Photos.Count != 0) {
                CopiedFlat.Preview = BitmapImageDecoder.GetDecodedBytes(Photos[0], 0, 100);
                CopiedFlat.Album.PhotoCollection = BinarySerializer.Serialize(Photos);
            }
            if (FieldFillness.IsFilled(CopiedFlat)) {
                if (isNew) {
                    Client.AddFlat(CopiedFlat);
                } else {
                    Client.UpdateFlat(CopiedFlat);
                }
                (obj as Window).Close();
            }
        }));
        
        public Flat CopiedFlat {
            get => copiedFlat;
            set {
                copiedFlat = value;
                OnPropertyChanged();
            }
        }
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
        public Flat OriginalFlat {
            get => originalFlat; 
            set => originalFlat = value;
        }
    }
}
