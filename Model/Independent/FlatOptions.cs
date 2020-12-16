using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    public class FlatOptions
    {
        readonly List<string> types = new List<string>() {
            "Гостинка",
            "Изолированная",
            "Общежитие",
            "Коммуналка",
            "Комната",
            "Коттедж",
        };
        readonly List<string> funds = new List<string>() {
            "Улучшенный",
            "Новый",
            "Хрущевка",
            "Сталинка",
            "Старый",
            "Новостройка",
            "Элитный",
        };
        readonly List<string> materials = new List<string>() {
            "Кирпичный",
            "Панельный",
            "Монолитный",
            "Монолитно-кирпичный",
            "Каркасно-монолитный",
            "Панельно-кирпичный",
            "Блочный",
        };
        readonly List<string> conditions = new List<string>() {
            "Евро",
            "Отличное",
            "Хорошее",
            "Жилое",
            "Требуется косметический ремонт",
            "Требуется капитальный ремонт",
            "Без внутренней отделки",
        };
        readonly List<string> floors = new List<string>() {
            "ДВП",
            "ДСП",
            "Деревянный",
            "Ковролин",
            "Ламинат",
            "Линолеум",
            "Паркет",
            "Стяжка",
        };
        readonly List<string> loggias = new List<string>() {
            "Нет",
            "Застекленная",
            "Не застекленная",
        };
        readonly List<string> bathrooms = new List<string>() {
            "Нет",
            "Раздельный",
            "Совмещенный",
            "Два",
            "Три и более",
        };
        readonly List<string> conveniences = new List<string>() {
            "Нет",
            "Все",
            "Частичные",
        };
        readonly List<string> heatings = new List<string>() {
            "Нет",
            "АГВ",
            "Котел",
            "Местное",
            "Печное",
            "ТЭЦ",
            "Форсунка",
            "Электро",
        };
        readonly List<string> waters = new List<string>() {
            "Нет",
            "ТЭЦ",
            "Котельная",
            "Колонка",
            "Котел",
            "Бойлер",
            "АГВ",
            "Электро",
        };
        readonly List<string> baths = new List<string>() {
            "Нет",
            "Обычная",
            "Сидячая",
            "Душ",
            "Душевая кабинка",
            "Джакузи",
        };
        readonly List<String> balconies = new List<string>() {
            "Нет",
            "Консольные балки",
            "Консольная плита",
            "Внешние опоры",
        };
        readonly List<string> multipliers = new List<string>() {
            "Тыс. руб",
            "Млн. руб"
        };
        public List<string> Types => types;
        public List<string> Funds => funds;
        public List<string> Materials => materials;
        public List<string> Conditions => conditions;
        public List<string> Floors => floors;
        public List<string> Loggias => loggias;
        public List<string> Bathrooms => bathrooms;
        public List<string> Conveniences => conveniences;
        public List<string> Heatings => heatings;
        public List<string> Waters => waters;
        public List<string> Baths => baths;
        public List<string> Balconies => balconies;
        public List<string> Multipliers => multipliers;
    }
}
