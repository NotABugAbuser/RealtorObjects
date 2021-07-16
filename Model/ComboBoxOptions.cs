using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealtorObjects.Model
{
    public class ComboBoxOptions {
        readonly int[] years = {
            1980,
            1981,
            1982,
            1983,
            1984,
            1985,
            1986,
            1987,
            1988,
            1989,
            1990,
            1991,
            1992,
            1993,
            1994,
            1995,
            1996,
            1997,
            1998,
            1999,
            2000,
            2001,
            2002,
            2003,
            2004,
            2005,
            2006,
            2007,
            2008,
            2009,
            2010,
            2011,
            2012,
            2013,
            2014,
            2015,
            2016,
            2017,
            2018,
            2019,
            2020,
            2021,
            2022,
            2023,
            2024,
            2025,
            2026,
            2027,
            2028,
            2029,
            2030,
            2031,
            2032,
            2033,
            2034,
            2035,
            2036,
            2037,
            2038,
            2039,
            2040,
            2041,
            2042,
            2043,
            2044,
            2045,
            2046,
            2047,
            2048,
            2049,
            2050
        };
        readonly int[] levelCount = {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            16,
            17,
            18,
            19,
            20,
            21,
            22,
            23,
            24,
            25,
            26,
            27,
            28,
            29,
            30,
            31,
            32,
            33,
            34,
            35,
            36,
            37,
            38,
            39,
            40,
            41,
            42,
            43,
            44,
            45,
            46,
            47,
            48,
            49,
            50
        };
        readonly int[] roomCount = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        readonly List<string> walls = new List<string>() {
            "Дерево",
            "Кирпич",
            "Монолит",
            "Панель",
            "Саман",
            "Саман + кирпич",
            "Шелев",
            "Шлак",
        };
        readonly List<string> categories = new List<string>() {
            "Земли сельскохозяйственного назначения",
            "Земли поселений",
            "Земли промышленности",
            "Земли особо охраняемой территории",
            "Земли лесного фонда",
            "Земли водного фонда",
            "Земли запаса",
        };
        readonly List<string> demarcations = new List<string>() {
            "Да",
            "В процессе",
            "Нет",
        };
        readonly List<string> roofs = new List<string>() {
            "Металлочерепица",
            "Мягкая кровля",
            "Ондулин",
            "Профильный настил",
            "Черепица",
            "Шифер",
        };
        readonly List<string> gases = new List<string>() { 
            "Есть",
            "Нет",
            "По участку",
            "По меже",
            "Рядом",
        };
        readonly List<string> houseWaters = new List<string>() {
            "Нет",
            "В доме",
            "Колонка",
            "Скважина",
            "По участку",
            "По меже",
            "Рядом",
        };
        readonly List<string> yards = new List<string>() { 
            "Общий",
            "Свой",
            "Нет",
        };
        readonly List<string> sewers = new List<string>() {
            "Центральная",
            "Нет",
            "Яма",
            "По меже",
            "По участку",
            "Рядом",
        };
        readonly List<string> houseTypes = new List<string>() {
            "Дом",
            "Земельный участок",
        };
        readonly List<string> districts = new List<string>(){
            "Авиагородок",
            "Авиаторов сквер",
            "Азовский",
            "Ветлечебница",
            "ВЖМ",
            "Гайдара",
            "Залесье",
            "Заря",
            "ЗЖМ",
            "Наливная",
            "Пальмира",
            "ПЧЛ",
            "РДВС",
            "СЖМ",
            "Солёное озеро",
            "Солнечный",
            "Центр"
        };
        readonly List<String> cities = new List<string>() {
            "Азов",
            "Батайск",
            "Весна",
            "Донская чаша",
            "Дружба",
            "Красный сад",
            "Кулешовка",
            "Лесная полянка",
            "Луч",
            "Милиоратор",
            "Мокрый Батай",
            "Овощной",
            "Ромашка",
            "Ростов-на-Дону",
            "Труд",
            "Ягодка"
        };
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
        readonly List<string> balconies = new List<string>() {
            "Нет",
            "Застекленная",
            "Не застекленная",
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
        public int[] Years => years;
        public int[] LevelCount => levelCount;
        public int[] RoomCount => roomCount;
        public List<string> Districts => districts;
        public List<string> Cities => cities;
        public List<string> HouseTypes => houseTypes;
        public List<string> Walls => walls;
        public List<string> Categories => categories;
        public List<string> Demarcations => demarcations;
        public List<string> Roofs => roofs;
        public List<string> Gases => gases;
        public List<string> HouseWaters => houseWaters;
        public List<string> Yards => yards;
        public List<string> Sewers => sewers;
    }
}
