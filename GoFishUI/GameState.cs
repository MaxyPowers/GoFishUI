using ChotoProCard;
using GoFish;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoDecks;

namespace GoFish
{
    /// <summary>
    /// Класс описывает состояние игры
    /// </summary>
    public class GameState
    {
        public readonly IEnumerable<Player> Players;        //Содержит перечисление всех игроков
        public readonly IEnumerable<Player> Opponents;      //Содержит всех игроков противников 
        public readonly Player HumanPlayer;                 //Игрок человек
        public bool GameOver { get; private set; } = false; //Статус игры, false - игра продолжается, true - игра оконченна
        public readonly Deck Stock;                         //Колода с которой проводится игра

        /// <summary>
        /// Конструктор создаёт игроков и раздаёт им их первые карты
        /// </summary>
        /// <param name="humanPlayerName">Имя игрока чееловека</param>
        /// <param name="opponetNames">Имена компьютеерных игроков</param>
        /// <param name="stock">Перемешанная колода для раздачи</param>
        public GameState (string humanPlayerName, IEnumerable<string> opponetNames, Deck stock)
        {
            HumanPlayer = new Player(humanPlayerName);                      //Создать игрока человека
            Opponents = (from name in opponetNames                          //Создать всех игроков противников
                      select new Player(name)).ToList();                    //и сохранить их в список
            Players = new List<Player>() { HumanPlayer }.Concat(Opponents); //Объеденить игроков в один список всех игроков
            Stock = stock;                                                  //Инициализировать колоду карт
            foreach (var player in Players)                                 //Выдать каждому игроку стартовый набор карт
            {
                player.GetNextHand(Stock);
            }
        }

        /// <summary>
        /// Получает случайного игрока, не совпадающего с текущим игроком
        /// </summary>
        /// <param name="currentPlayer">Текущий игрок</param>
        /// <returns>Случайный игрок у которого текущий игрок может попросить карту</returns>
        public Player RandomPlayer(Player currentPlayer)
        {
            var otherPlayers = Players.Where(p => p != currentPlayer).ToList(); //Выбрать из списка всех игроков не совпадающих с текущим игроком
            int indexOfRandomPlayer = Player.Random.Next(otherPlayers.Count()); //Получить случайный индекс, для выбора случайного игрока
            return otherPlayers[indexOfRandomPlayer];                           //Вернуть случайного игрока не совпадающего с текущим
        }

        /// <summary>
        /// Осуществляет один игровой ход
        /// </summary>
        /// <param name="player">Игрок запрашивающий карту</param>
        /// <param name="playerToAsk">Игрок у которого спрашивают карту</param>
        /// <param name="valueToAskFor">Значени карты которое спрашивают</param>
        /// <param name="stock">Колода для взятия карт</param>
        /// <returns>Сообщение описывающее произошедшее</returns>
        public string PlayRound(Player player, Player playerToAsk, Values valueToAskFor, Deck stock)
        {
                                                           //Сообщение: Кто у кого спрашивает какую карту
            var message = $"{player.Name} спрашивает: {playerToAsk.Name} есть ли у тебя {valueToAskFor}?{Environment.NewLine}";
                                                           //Спросить и получить карты
            var askedCard = playerToAsk.DoYouHaveAny(valueToAskFor, stock);
            
            if(askedCard.Count() > 0)                      //Если были полчучены карты от игрока противника
            {
                player.AddCardsAndPullOutBooks(askedCard); //Игрок который спрашивал карты, добаляет их себе в руку и откладывает книги
                                                           //Добавить к сообщению информацию о выполненом ходе
                message += $"{playerToAsk} имеет {askedCard.Count()} {(askedCard.Count() > 1 ? "такие" : "такую")} карт{(askedCard.Count() > 1 ? "ы" : "у")}";
            }
            else if(stock.Count() == 0)                    //Если в колоде закончились карты
            {
                message += "В колоде закончились карты";   //Добавить в сообщение информацию о пустой колоде
            }
            else                                           //Если карты не были получены и колода не пуста
            {
                player.DrawCard(stock);                    //Игрок спрашивающий карту, берёт карту из колоды
                message += $"{player.Name} берёт карту";   //Добавить в сообщение информацию, о том что игрок взял карту
            }
            if(player.Hand.Count() == 0)                   //Если у игрока нет карт в руке
            {
                player.GetNextHand(stock);                 //Раздать игроку карты
                message += $"{Environment.NewLine}{player.Name} раздал все свои карты, и взял {player.Hand.Count()} карт{(player.Hand.Count() > 1 ? "ы" : "у")} из колоды";             
                                                           //Добавить сообщение о том что игрок взял карты
            }
            return message;                                //Вернуть сообщение с рузультатом раунда
        }

        /// <summary>
        /// Проверяет наличие победителя
        /// </summary>
        /// <returns>Строка с именами победителя, пустая строка если победителей нет</returns>
        public string CheckForWinner()
        {
            var playerCards = Players.Select(player => player.Hand.Count()).Sum();           //Получить общее количество карт в руках всех игроков
            if (playerCards > 0) return null;                                                //Если сумма больше 0, вернуть null и выйти из метода
            GameOver = true;                                                                 //Игра окончена
            var winningBookCount = Players.Select(player => player.Books.Count()).Max();     //Найти максимальное значение количества книг среди игроков
            var winners = Players.Where(player => player.Books.Count() == winningBookCount); //Найти игрока/ов с максимальным количеством книг
            if (winners.Count() == 1) return $"Победитель {winners.First().Name}";           //Если победитель один, вернуть строку с его объявлением
            return $"Победители: {string.Join(" и ", winners)}";                             //Если победителей несколько, вернуть строку с их объявлением
        }
    }
}
