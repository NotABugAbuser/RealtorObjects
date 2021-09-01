using BitmapImageDecoding;
using Microsoft.Win32;
using RealtorObjects.Model;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    public class FlatFormVM : RealtorObjectFormVM
    {
        private Flat copiedFlat = new Flat();
        private Flat originalFlat = new Flat();
        private int currentAgentId = 0;
        public FlatFormVM() {
        }
        public FlatFormVM(string agentName, int agentId) {
            isNew = true;
            Title = $"[Квартирный объект]  —  Добавление";
            currentAgent = agentName;
            CanEdit = true;
            if (Debugger.IsAttached) {
                CopiedFlat = Flat.CreateTestFlat();
            } else {
                CopiedFlat = new Flat();
            }
            CopiedFlat.AgentId = agentId;
            CopiedFlat.Agent = agentName;
        }
        public FlatFormVM(Flat flat, string agentName, int currentAgentId) {
            CopiedFlat = flat.GetCopy();
            OriginalFlat = flat;
            Title = $"[#{CopiedFlat.Id}] [Тип: {CopiedFlat.GeneralInfo.ObjectType}] [Создатель заявки: {CopiedFlat.Agent}]  —  Просмотр";
            currentAgent = agentName;
            this.currentAgentId = currentAgentId;
        }
        public CustomCommand AllowToEdit => allowToEdit ?? (allowToEdit = new CustomCommand(obj => {
            if (CheckAccess(CopiedFlat.Agent, currentAgent, CopiedFlat.AgentId, currentAgentId)) {
                CanEdit = true;
                Title = $"[#{CopiedFlat.Id}] [Тип: {CopiedFlat.GeneralInfo.ObjectType}] [Создатель заявки: {CopiedFlat.Agent}]  —  Редактирование";
            }
        }));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj => {
            if (Photos.Count != 0) {
                CopiedFlat.Preview = BitmapImageDecoder.GetDecodedBytes(Photos[0], 0, 100);
                CopiedFlat.Album.PhotoCollection = BinarySerializer.Serialize(Photos);
            }
            if (CopiedFlat.Album.PhotoCollection.Length < 1100) {
                CopiedFlat.Album.PhotoCollection = Array.Empty<byte>();
            }
            if (CopiedFlat.GeneralInfo.RoomCount == 1) {
                CopiedFlat.GeneralInfo.ObjectType = "Комната";
            } else {
                CopiedFlat.GeneralInfo.ObjectType = "Квартира";
            }
            if (FieldFillness.IsFilled(CopiedFlat) && Client.CanConnect()) {
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
