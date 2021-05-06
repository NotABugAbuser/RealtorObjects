using RealtyModel.Model.Derived;
using System;
using System.Collections.Generic;
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
            " — Количество квартир"
        };
        StringBuilder message = new StringBuilder("Для продолжения требуется заполнить следуюшие поля:");
        public bool CheckFieldsOfFlat(Flat flat) {
            for (int i = 1; i < fields.Count; i++) {
                int j = i - 1;
                if (fields[j] is string str) {
                    if (String.IsNullOrEmpty(str)) {
                        isEveryFieldFilled = false;
                        message.Append(fieldsForMessage[j]);
                    }
                }
            }
            return true;
        }
        public FieldChecking() { }
        public FieldChecking(Flat flat) {
            this.flat = flat;
            fields.Add(flat.Customer.Name);
            fields.Add(flat.Location.FlatNumber);
        }
    }
}
