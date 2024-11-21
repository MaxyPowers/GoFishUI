using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using ChotoProCard;
using System.Runtime.CompilerServices;
using CardComparer;
using GoFish;

namespace TwoDecks
{
    /// <summary>
    /// Класс "Колода(карт)". Класс содержит методы для работы с колодой. Колода содержит 52 карты.
    /// </summary>
   public  class Deck : ObservableCollection<Card>
    {
        /// <summary>
        /// Поле содержит объект "Рандом", инициализирован объектом рандом из класса "Игрок" во избежание
        /// генерации одинаковых случайных чисел в друх разных классах
        /// </summary>
        private static Random rnd = Player.Random;

        /// <summary>
        /// Конструктор вызывает метот "Сброса"
        /// </summary>
        public Deck() 
        {
            Reset();
        }

        /// <summary>
        /// Метот очищает текущее перечисление колоды и добаляет новые 52 карты по очереди
        /// </summary>
        public void Reset()
        {
            Clear(); //Очистить колоду
            for (int suit = 0; suit <= 3; suit++)                //Перебрать масти карт
            {
                for (int value = 1; value <= 13; value++)        //Перебрать номиналы карт
                {
                    Add(new Card((Values) value, (Suits) suit)); //Добавить в перечисление карту с мастью и номиналом равными текущим итераторам
                }
            }
        }

        /// <summary>
        /// Метод выдаёт карту из колоды по индексу
        /// </summary>
        /// <param name="index">Индекс(номер карты в колоде)</param>
        /// <returns>Запрашиваемая карта</returns>
        public Card Deal(int index)
        {
            Card card = base[index]; //Взять карту по индексу из колоды и сохранить в отдельную переменную
            RemoveAt(index);         //Удалить выбранную карту из колоды
            return card;             //Вернуть выбранную карту
        }

        /// <summary>
        /// Метод тасует текущую колоду карт
        /// </summary>
        /// <returns>Перетасованноя колода карт</returns>
        public Deck Shuffle()
        {
            List<Card> copyDeck = new List<Card>(this); //Скопировать текующе колоду карт в новый список
            Clear();                                    //Очистить текущую колоду карт
            while (copyDeck.Count > 0)                  //Выполнять пока есть карты в скопированной колоде
            {
                int index = rnd.Next(copyDeck.Count);   //Выбрать случайный индекс (не должен привышать количества карт в колоде)
                Card card = copyDeck[index];            //Сохранить в отдельную переменную карту со случайным индексом из скопированной колоды
                copyDeck.RemoveAt(index);               //Удалить выбранную карту из колоды
                Add(card);                              //Добавить выбранную карту в текущую колоду
            }

            return this;                                //Вернуть текущее перетасованное перечисление карт
        }

        /// <summary>
        /// Отсортировать колоду карт по масти и номиналу
        /// </summary>
        public void Sort()
        {
            List<Card> sortedCard = new List<Card>(this); //Скопировать текущую колоду в новое перечисление карт
            sortedCard.Sort(new CardComparerByValue());   //Отсортировать перечисление карт
            Clear();                                      //Очистить текущую колоду
            ///Добавить карты из отсортированного перечисления в текущее
            foreach (Card card in sortedCard)
            {
                Add(card); 
            }
        }
    }
}
