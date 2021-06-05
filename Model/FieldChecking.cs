using RealtyModel.Model.Derived;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RealtorObjects.Model
{
    class FieldChecking
    {
        private Flat flat;
        bool isEveryFieldFilled = true;
        List<object> fields = new List<object>();
        string[] fieldsForMessage = new string[] {
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
            " — Кв-л",
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
            " — Окна",
            " — Квадратные метры",
            " — Цена",
            " — Фотографии",
        };
        public bool CheckFieldsOfFlat() {
            StringBuilder message = new StringBuilder("Для продолжения требуется заполнить следующие поля:");
            isEveryFieldFilled = true;
            for (int i = 0; i < fields.Count; i++) {
                if (fields[i] is string str) {
                    if (String.IsNullOrEmpty(str) || str == "") {
                        isEveryFieldFilled = false;
                        message.Append($"\n{fieldsForMessage[i]}");
                    }
                } else if (fields[i] is int number) {
                    if (number < 1) {
                        isEveryFieldFilled = false;
                        message.Append($"\n{fieldsForMessage[i]}");
                    }
                } else if (fields[i] is float fl) {
                    if (fl < 0.001) {
                        isEveryFieldFilled = false;
                        message.Append($"\n{fieldsForMessage[i]}");
                    }
                } else if (fields[i] is null) {
                    isEveryFieldFilled = false;
                    message.Append($"\n{fieldsForMessage[i]}");
                } else if (fields[i] is short sh) {
                    if (sh < 1) {
                        isEveryFieldFilled = false;
                        message.Append($"\n{fieldsForMessage[i]}");
                    }
                }
            }
            if (!isEveryFieldFilled) {
                MessageBox.Show(message.ToString());
            }
            return isEveryFieldFilled;
        }
        public FieldChecking() {
        }
        public FieldChecking(Flat flat) {
            this.flat = flat;
            fields.Add(flat.CustomerName);
            fields.Add(flat.CustomerPhoneNumbers);
            fields.Add(flat.Location.City);
            fields.Add(flat.Location.District);
            fields.Add(flat.Location.Street.Name);
            fields.Add(flat.Location.FlatNumber);
            fields.Add(flat.Location.HouseNumber);
            fields.Add(flat.Info.Material);
            fields.Add(flat.Info.Fund);
            fields.Add(flat.GeneralInfo.RoomCount);
            fields.Add(flat.Info.Type);
            fields.Add(flat.GeneralInfo.General);
            fields.Add(flat.GeneralInfo.Living);
            fields.Add(flat.GeneralInfo.Kitchen);
            fields.Add(flat.GeneralInfo.Condition);
            fields.Add(flat.Info.Floor);
            fields.Add(flat.GeneralInfo.Ceiling);
            fields.Add(flat.Info.Loggia);
            fields.Add(flat.Info.Balcony);
            fields.Add(flat.Info.Bathroom);
            fields.Add(flat.GeneralInfo.Convenience);
            fields.Add(flat.GeneralInfo.Heating);
            fields.Add(flat.GeneralInfo.Water);
            fields.Add(flat.Info.Bath);
            fields.Add(flat.Info.Windows);
            fields.Add(flat.Cost.Price);
            fields.Add(flat.Album.PhotoCollection.Length);
        }
    }
}
