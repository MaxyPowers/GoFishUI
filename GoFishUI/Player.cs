using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChotoProCard;
using CardComparer;
using TwoDecks;

namespace GoFish
{
    /// <summary>
    /// Класс описывает игрока. Содержит методы и свойства для игрока в "GoFish"
    /// </summary>
    public class Player
    {
        public static Random Random = new Random();
        /// <summary>
        /// Содержит список карт в руке игрока
        /// </summary>
        private List<Card> hand = new List<Card>();
        /// <summary>
        /// Содержит список энамов номиналов карты, комлект которых игрок уже собрал
        /// </summary>
        private List<Values> books = new List<Values>();

        /// <summary>
        /// Карты в руке игрока
        /// </summary>
        public IEnumerable<Card> Hand => hand;

        /// <summary>
        /// Комплекты карт, которые игрок уже собрал
        /// </summary>
        public IEnumerable<Values> Books => books;

        public readonly string Name;

        /// <summary>
        /// Изменяет окончание множественного числа в зависимости от количества карт
        /// </summary>
        public static string PluralHand(int s)
        {
            if ((s == 0 || s >= 5) && (s < 21 || s >= 25) && (s < 31 || s >= 35) && (s < 41 || s >= 45) && (s < 51))
                return string.Empty;
            else if (s == 1 || s == 21 || s == 31 || s == 41 || s == 51) //Жуть какая-то
                return "у";
            else if ((s > 1 || s >= 22) && (s < 25 || s >= 32) && (s >= 42 || s < 45) || s == 52)
                return "ы";
            else
                return "";

        }

        /// <summary>
        /// Изменяет кокончание множественного числа в зависимости от количества книг
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string PluralBook(int s)
        {
            if (s == 0 || s >= 5)
                return string.Empty;
            else if (s == 1)
                return "у";                                              //Тоже жуть, но выглядит проще
            else if (s >= 2)
                return "и";
            else
                return "";
        }

        /// <summary>
        /// Возвращает текущий статус игрока: количество карт, книг
        /// </summary>
        public string Status => $"{Name} имеет {hand.Count} карт{PluralHand(hand.Count)} и {books.Count} книг{PluralBook(books.Count)}";

        /// <summary>
        /// Конструктор для создания игрока
        /// </summary>
        /// <param name="name">Имя игрока</param>
        public Player(string name) => Name = name;

        /// <summary>
        /// Альтернативный конструктор для модульного тестирования
        /// </summary>
        /// <param name="name">Имя игрока</param>
        /// <param name="cards">Набор начальных карт</param>
        public Player(string name, IEnumerable<Card> cards)
        {
            Name = name;
            hand.AddRange(cards);
        }

        /// <summary>
        /// Получает до пяти карт из колоды
        /// </summary>
        /// <param name="stock">Колода из которой берётся следующая раздача</param>
        public void GetNextHand(Deck stock)
        {
            while (stock.Count > 0 && hand.Count < 5)
                hand.Add(stock.Deal(0));
        }

        /// <summary>
        /// Если у меня есть карты соответствующего значения, вернуть их. Если карты закончились взять новую раздачу из колоды.
        /// </summary>
        /// <param name="value">Значение которое спрашивается</param>
        /// <param name="deck">Колода для следующей раздачи</param>
        /// <returns>Карты, взятые из руки другого игрока</returns>
        public IEnumerable<Card> DoYouHaveAny(Values value, Deck deck)
        {
            var mathingCards =              //Сохранить все подходящие по номиналу карты
                from card in hand           //Карты отбираются из "руки"
                where card.Value == value   //Проверяется значение карты
                orderby card.Suit           //Карты сортируются по масти
                select card;                //Сохранить выбранную карту

            hand = (from card in hand       //Убрать из руки выбранные карты
                   where card.Value != value//Отобрать из руки только те карты которые не совпадают с запрашиваемым номиналом
                   select card).ToList();   //и оставить только из в руке
            
            if (hand.Count == 0 )           //Если в руке не осталось карт, взять из колоды 
                GetNextHand(deck);

            return mathingCards;            //Вернуть выбраные карты
        }

        /// <summary>
        /// Когда игрок получает карты от другого игрока, он добавляет их в свою руку и выкладывает все подходящие книги
        /// </summary>
        /// <param name="cards">Карты добавляемые из другой руки</param>
        public void AddCardsAndPullOutBooks(IEnumerable<Card> cards)
        {
            hand.AddRange(cards);                           //Добавить в руку карты

            var foundBooks = hand                           //Отобрать готовые наборы из руки
                .GroupBy(card => card.Value)                //Сгрупировать карты по номиналу
                .Where(group => group.Count() == 4)         //Группа должан содержать 4 карты
                .Select(group => group.Key);                //Номинал карты - ключь группы

            books.AddRange(foundBooks);                     //Добавить выбранные книги
            books.Sort();                                   //Отсортировать выбранные книги

            hand = hand                                     //Убрать из руки карты которые были помещены в книги
                .Where(card => !books.Contains(card.Value)) //Выбрать карты которые не совпадают с отложенными в книги
                .ToList();                                  //и оставить их в руке
        }

        /// <summary>
        /// Взять карту из колоды и добавить в руку игрока
        /// </summary>
        /// <param name="stock">Колода из которой берётся карта</param>
        public void DrawCard(Deck stock)
        {
            if (stock.Count > 0)                                            //Если в колоде есть карты, взять карту и выложить книги
                AddCardsAndPullOutBooks(new List<Card>() { stock.Deal(0) });
        }

        /// <summary>
        /// Выбрать случайный номинал из руки игрока
        /// </summary>
        /// <returns>Значение случайно выбраной карты из руки игрока</returns>
        public Values RandomValueFromHand() => hand.OrderBy(card => card.Value) //Отсортировать карты по номиналу
            .Select(card => card.Value)                                         //Выбрать карты по номиналу
            .Skip(Random.Next(hand.Count))                                      //Пропустить случайное количество карт
            .First();                                                           //Взять первое значение

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Возвращает имя игрока, количество карт и книг</returns>
        public override string ToString() => Name;
    }
}
