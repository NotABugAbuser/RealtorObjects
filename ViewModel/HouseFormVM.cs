using BitmapImageDecoding;
using MiscUtil;
using RealtorObjects.Model;
using RealtyModel.Model;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;
using RealtyModel.Model.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.ViewModel
{
    public class HouseFormVM : RealtorObjectFormVM
    {
        private House originalHouse = new House();
        private House copiedHouse = new House();
        private int currentAgentId = 0;
        public HouseFormVM() {
        }
        public HouseFormVM(string agentName, int agentId) {
            isNew = true;
            Title = $"[Земельный объект]  —  Добавление";
            currentAgent = agentName;
            CanEdit = true;
            if (Debugger.IsAttached) {
                CopiedHouse = House.CreateTestHouse();
            } else {
                CopiedHouse = new House();
            }
            CopiedHouse.AgentId = agentId;
            CopiedHouse.Agent = agentName;
        }
        public HouseFormVM(House house, string agentName, int currentAgentId) {
            CopiedHouse = house.GetCopy();
            OriginalHouse = house;
            Title = $"[#{CopiedHouse.Id}] [Тип: {CopiedHouse.GeneralInfo.ObjectType}] [Создатель заявки: {CopiedHouse.Agent}]  —  Просмотр";
            currentAgent = agentName;
            this.currentAgentId = currentAgentId;
        }
        public CustomCommand AllowToEdit => allowToEdit ?? (allowToEdit = new CustomCommand(obj => {
            if (CheckAccess(CopiedHouse.Agent, currentAgent, CopiedHouse.AgentId, currentAgentId)) {
                CanEdit = true;
                Title = $"[#{CopiedHouse.Id}] [Тип: {CopiedHouse.GeneralInfo.ObjectType}] [Создатель заявки: {CopiedHouse.Agent}]  —  Редактирование";
            }
        }));
        public CustomCommand Confirm => confirm ?? (confirm = new CustomCommand(obj => {
            CopiedHouse.Album.PhotoCollection = BinarySerializer.Serialize(Photos);
            if (Photos.Count != 0) {
                CopiedHouse.Preview = BitmapImageDecoder.GetDecodedBytes(Photos[0], 0, 100);
            }
            if(CopiedHouse.Album.PhotoCollection.Length < 1100) {
                CopiedHouse.Album.PhotoCollection = Array.Empty<byte>();
            }
            if (FieldFillness.IsFilled(CopiedHouse) && Client.CanConnect()) {
                if (isNew) {
                    Client.AddHouse(CopiedHouse);
                } else {
                    Client.UpdateHouse(CopiedHouse);
                }
                (obj as Window).Close();
            }
        }));
        public House OriginalHouse {
            get => originalHouse;
            set => originalHouse = value;
        }
        public House CopiedHouse {
            get => copiedHouse;
            set => copiedHouse = value;
        }
    }
}
