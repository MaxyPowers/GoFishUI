using ChotoProCard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoDecks;

namespace GoFish
{
    /// <summary>
    /// Игровой контроллер управляет процессом игры
    /// </summary>
    public class GameController
    {
        private GameState gameState;                                                    //Объект состояния игры
        public bool GameOver { get { return gameState.GameOver; } }                     //Состояние окончания игры
        public Player HumanPlayer { get { return gameState.HumanPlayer; } }             //Игрок человек
        public IEnumerable<Player> Opponents { get { return gameState.Opponents; } }    //игроки противники
        public string Status { get; private set; }                                      //Статус игры
        public string WinnerStatus {  get; private set; }                               //Статус окончания игры и определение победителей

        /// <summary>
        /// Конструктор стоздаёт новый GameController
        /// </summary>
        /// <param name="humanPlayerName">Имя игрока человека</param>
        /// <param name="computerPlayerNames">Имена компьютерных игроков</param>
        public GameController(string humanPlayerName, IEnumerable<string> computerPlayerNames)
        {
            //Инициализировать объект Состояние Игры с игрококм человеком, компьютерными игроками и новой перетасованной колодой
            gameState = new GameState(humanPlayerName, computerPlayerNames, new Deck().Shuffle());
            //Обновить статус, сообщением о начале новой игры с именами всех игроков
            Status = $"Начинается новая игра с игроками: {string.Join(", ", gameState.Players)}";
        }

        /// <summary>
        /// Проводит следующий раунд, завершает игру если у всех закончились карты
        /// </summary>
        /// <param name="playerToAsk">Игрок у которого человек спрашивает карту</param>
        /// <param name="valueToAskFor">Значение карты которую спрашивает игрок</param>
        public void NextRound(Player playerToAsk, Values valueToAskFor)
        {
            //Игрок человек ходит первым
            //Обновить статус сообщением о результатах первого раунда
            Status = gameState.PlayRound(gameState.HumanPlayer, playerToAsk, valueToAskFor, gameState.Stock) + Environment.NewLine;
            ComputerPlayersPlayNextRound();                  //Компьютерные игроки должны сделать свои ходы
                                                             //Добавить в статус информацию о колоде
            Status += $"{Environment.NewLine}Колода содержит {gameState.Stock.Count()} карт{Player.PluralHand(gameState.Stock.Count())}";
            WinnerStatus = gameState.CheckForWinner();       //Проверить наличие победителей, добавить в статус
        }

        /// <summary>
        /// Все компьютерные игроки у которых есть карты, играют следующий раунд.
        /// Если у человека закончились карты, колода заканчивается и он доигрывает оставшуюся часть игры.
        /// </summary>
        private void ComputerPlayersPlayNextRound()
        {
            IEnumerable<Player> computerPlayersWithCards;               //Перечисление всех компьютерных игроков
            do
            {
                                                                        //Выбрать всех компьютерных игкроков у которых есть карты
                computerPlayersWithCards = gameState.Opponents.Where(player => player.Hand.Count() > 0);
                foreach (Player player in computerPlayersWithCards)     //Выбрать игрока который делает ход
                {
                    var randomPlayer = gameState.RandomPlayer(player);  //Выбрать случайного игрока, против которого будет ход
                    var randomValue = player.RandomValueFromHand();     //Выбрать случайный номинал карты из руки игрока для запроса
                                                                        //Сделать ход, обновить стасту, резальтатом хода
                    Status += gameState.PlayRound(player, randomPlayer, randomValue, gameState.Stock) + Environment.NewLine;
                }
            } while (gameState.HumanPlayer.Hand.Count() == 0 && computerPlayersWithCards.Count() > 0);//Повторять пока компьютеры имеют карты а человек нет
        }
        /// <summary>
        /// Запуск новой игры с теми же именами игроков
        /// </summary>
        public void NewGame()
        {
            Status = "Начинается новая игра"; //Обновить статус сообщением о начале новой игры
                                              //Инициализировать новый объект Состояние Игры с теми же аргументамы что использовались в игры
            gameState = new GameState(gameState.HumanPlayer.Name, gameState.Opponents.Select(player => player.Name), new Deck().Shuffle());
        }
    }
}
