using RealtyModel.Model.Base;
using RealtyModel.Model.Derived;
using RealtyModel.Model.Operations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.Model
{
    class FieldFillness
    {
        private static readonly string[] flatMessages = new string[] {
            " — Заказчик",
            " — Телефоны",
            " — Город",
            " — Район",
            " — Улица",

            " — Квартира №",
            " — Дом №",
            " — Материал",
            " — Фонд",
            " — Количество комнат",
            
            " — Тип",
            " — Общая",
            " — Жилая",
            " — Кухня",
            " — Состояние",
            
            " — Полы",
            " — Потолок",
            " — Лоджия",
            " — Балкон",
            " — Сан. узел",
            
            " — Удобства",
            " — Отопление",
            " — Горячая вода",
            " — Ванна",
            " — Цена",
            
            " — Фотографии",
            };
        private static readonly string[] houseMessages = new string[] { 
            " — Заказчик",
            " — Телефоны",
            " — Город",
            " — Район",
            " — Улица",

            " — Дом №",
            " — Крыша",
            " — Стены",
            " — Межевание",
            " — Комнаты",

            " — Год",
            " — Фасад",
            " — Этажей",
            " — Общая",
            " — Жилая",

            " — Кухня",
            " — Вода",
            " — Двор",
            " — Газ",
            " — Состояние",

            " — Удобства",
            " — Отопление",
            " — Канализация",
            " — Категория",
            " — Тип объекта",

            " — Потолок",
            " — Сотки",
            " — Гектары",
            " — Цена",
            " — Фотографии",
        };
        public static bool IsFilled(Flat flat) {
            return BuildNotification(GetFields(flat), flatMessages);
        }
        public static bool IsFilled(House house) {
            return BuildNotification(GetFields(house), houseMessages);
        }
        private static bool BuildNotification(List<object> fields, string[] messages) {
            bool isEveryFieldFilled = true;
            StringBuilder message = new StringBuilder("Для продолжения требуется заполнить следующие поля:");
            for (int i = 0; i < fields.Count; i++) {
                if (fields[i] is string str) {
                    if (String.IsNullOrEmpty(str) || str == "") {
                        isEveryFieldFilled = false;
                        message.Append($"\n{messages[i]}");
                    }
                } else if (fields[i] is int number) {
                    if (number < 1) {
                        isEveryFieldFilled = false;
                        message.Append($"\n{messages[i]}");
                    }
                } else if (fields[i] is float fl) {
                    if (fl < 0.001) {
                        isEveryFieldFilled = false;
                        message.Append($"\n{messages[i]}");
                    }
                } else if (fields[i] is null) {
                    isEveryFieldFilled = false;
                    message.Append($"\n{messages[i]}");
                } else if (fields[i] is short sh) {
                    if (sh < 1) {
                        isEveryFieldFilled = false;
                        message.Append($"\n{messages[i]}");
                    }
                }
            }
            if (!isEveryFieldFilled) {
                OperationNotification.WarningNotify(ErrorCode.NotFilled, message.ToString());
            }
            return isEveryFieldFilled;
        }
        private static List<object> GetFields(House house) {
            List<object> fields = new List<object> {
                house.CustomerName,
                house.CustomerPhoneNumbers,
                house.Location.City,
                house.Location.District,
                house.Location.Street,

                house.Location.HouseNumber,
                house.Info.Roof,
                house.Info.Walls,
                house.Info.Demarcation,
                house.GeneralInfo.RoomCount,

                house.GeneralInfo.Year,
                house.Info.FacadeLength,
                house.GeneralInfo.LevelCount,
                house.GeneralInfo.General,
                house.GeneralInfo.Living,

                house.GeneralInfo.Kitchen,
                house.GeneralInfo.Water,
                house.Info.Yard,
                house.Info.Gas,
                house.GeneralInfo.Condition,

                house.GeneralInfo.Convenience,
                house.GeneralInfo.Heating,
                house.Info.Sewerage,
                house.Info.EarthCategory,
                house.GeneralInfo.ObjectType,

                house.GeneralInfo.Ceiling,
                house.Info.Hundreds,
                house.Info.Hectar,
                house.Price,
                house.Album.PhotoCollection.Length
            };
            return fields;
        }
        private static List<object> GetFields(Flat flat) {
            List<object> fields = new List<object> {
                flat.CustomerName,
                flat.CustomerPhoneNumbers,
                flat.Location.City,
                flat.Location.District,
                flat.Location.Street,

                flat.Location.FlatNumber,
                flat.Location.HouseNumber,
                flat.Info.Material,
                flat.Info.Fund,
                flat.GeneralInfo.RoomCount,

                flat.Info.Type,
                flat.GeneralInfo.General,
                flat.GeneralInfo.Living,
                flat.GeneralInfo.Kitchen,
                flat.GeneralInfo.Condition,

                flat.Info.Floor,
                flat.GeneralInfo.Ceiling,
                flat.Info.Loggia,
                flat.Info.Balcony,
                flat.Info.Bathroom,

                flat.GeneralInfo.Convenience,
                flat.GeneralInfo.Heating,
                flat.GeneralInfo.Water,
                flat.Info.Bath,
                flat.Price,

                flat.Album.PhotoCollection.Length
            };
            return fields;
        }
    }
}
