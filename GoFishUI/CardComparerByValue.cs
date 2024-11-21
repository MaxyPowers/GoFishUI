using ChotoProCard;
using System;
using System.Collections.Generic;

/*
 * 
 */
namespace CardComparer
{
    /// <summary>
    /// Клас описывает объект игральной карты
    /// </summary>
    public class Card : IComparable<Card>
    {
        /// <summary>
        /// Сравнить текущую карту с другой
        /// </summary>
        /// <param name="other">Другая карта для сравнения</param>
        /// <returns>
        /// <para>1 если масть и/или номинал текущей карты больше</para>
        /// <para>-1 если масть и/или номинал текущей карты меньше</para>
        /// </returns>
        public int CompareTo(Card other)
        {
            return new CardComparerByValue().Compare(this, other);
        }

        public Values Value { get; private set; }                //Хранит номинал карты
        public Suits Suit { get; private set; }                  //Хранит масть карты
        public string Name { get { return $"{Value} {Suit}"; } } //Строковое представление карты

        /// <summary>
        /// Конструктор присваивает масть и номинал карты
        /// </summary>
        /// <param name="values">Номинал карты</param>
        /// <param name="suit">Масть карты</param>
        public Card(Values values, Suits suit)
        {
            Value = values;
            Suit = suit;
        }

        /// <summary>
        /// Возвращает строковое представление карты
        /// </summary>
        /// <returns>Номинал и масть карты</returns>
        public override string ToString()
        {
            return Name;
        }
    }

    class CardComparerByValue : IComparer<Card>
    {
        /// <summary>
        /// Сравнить карты по масти. Если масти одинаковые сравнить карты по номиналу.
        /// </summary>
        /// <param name="x">Карта с которой сравнивают</param>
        /// <param name="y">Карта которую сравнивают</param>
        /// <returns>
        /// <para>1 если масть первой карты больше</para>
        /// <para>-1 если масть первой карты меньше</para>
        /// <para>
        /// Если масти карт равны:
        /// <para>1 если номинал первой карты больше</para>
        /// <para>-1 если номинал первой карты меньше</para>
        /// <para>0 если номинал и масть карты совпадает</para>
        /// </para>
        /// </returns>
        public int Compare(Card x, Card y)
        {
            if (x.Suit < y.Suit) 
                return -1;
            else if (x.Suit > y.Suit)
                return 1;
            else
            {
                if (x.Value < y.Value)
                    return -1;
                else if (x.Value > y.Value)
                    return 1;
                else
                    return 0;
            }
        }
    }
}