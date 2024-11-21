using CardComparer;
using ChotoProCard;
using GoFish;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GoFishUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Управляет процессом игры
        /// </summary>
        private GameController gameController;

        public MainWindow()
        {
            InitializeComponent();
            ShowPlayerNameWindow(); //Показать окно конфигурации перед началом игры
        }

        /// <summary>
        /// Показывает окно конфигураций
        /// </summary>
        public void ShowPlayerNameWindow()
        {
            var playerNameWindow = new PlayerNameWindow(); //Создать объект нового окна
                
            if (playerNameWindow.ShowDialog() == true)
            {
                //Начать игру с полученными данными из окна конфигурации
                StartGame(playerNameWindow.PlayerName, playerNameWindow.OpponentCount);
            }
            else
            {
                MessageBox.Show("Имя игрока не было введено"); //Не используется
                Close();
            }
        }

        /// <summary>
        /// Начать игру
        /// </summary>
        /// <param name="playerName">Имя игрока человека</param>
        /// <param name="opponentCount">Количество компьютерных игроков</param>
        public void StartGame(string playerName, int opponentCount)
        {
            var allOpponentNames = Enum.GetValues(typeof(Opponents)).Cast<Opponents>().ToList(); //Получить все возможные имена противников
            var selectedOpponents = new List<string>();                                          //Новый список выбранных имён

            for (int i = 0; i < opponentCount; i++)
            {
                int index = Player.Random.Next(allOpponentNames.Count);     //Выбрать случайных индекс для выбора имени игрока
                selectedOpponents.Add(allOpponentNames[index].ToString());  //Добавить случайно выбранное имя в новый список
                allOpponentNames.RemoveAt(index);                           //Удалить выбранное имя из спика имён, во избежание дублирования
            }

            gameController = new GameController(playerName, selectedOpponents); //Инициализировать объект управляющий игрой
            UpdateStatus();                                                     //Обновить статус игры
        }

        /// <summary>
        /// Сыграть один раунд
        /// </summary>
        public void PlayRound()
        {
            if (gameController == null) //Если объект управляющий игрок отсутствует, выйти из метода
            {
                MessageBox.Show("По какой-то причине игра еще не началась");
                return;
            }
            //Имя игрока у которого запрашивается карта, берётся из выбраного элемента в ЛистБоксе с именами противников
            var playerToAsk = (Player)OpponentNamesList.SelectedItem; 
            //Запрашиваемая у противника карта берётся из выбранного элемента в ЛистБоксе с картами в руке игрока
            var valueToAsk = (Card)HumanPlayerHand.SelectedItem;      

            if(playerToAsk == null)     //Если игрок не выбрал противника, сообщить ему об этом и не продолжать игру
            {
                MessageBox.Show("Выберите игрока у которого хотите спросить карту");
                return;
            }
            else if (valueToAsk == null)//Если игрок не выбрал карту, сообщить ему об этом и не продолжать игру
            {
                MessageBox.Show("Выберите номинал карты, которую хотите спросить");
                return;
            }

            gameController.NextRound(playerToAsk, valueToAsk.Value); //Выполнить один игровой раунд
            UpdateStatus();                                          //Обновить статус игры
        }

        /// <summary>
        /// Обновляет элементы пользовательского интерфеса, актуальнными данными о состоянии игры
        /// </summary>
        public void UpdateStatus()
        {
            GameEventTextBlock.Text = gameController.Status;                    //Вывести события игры
            GameProgressTextBlock.Text = gameController.HumanPlayer.Status;     //Вывести состояние игрока человека
            foreach (var player in gameController.Opponents)                    //Вывести стояния компьютерных игроков
            {
                GameProgressTextBlock.Text += Environment.NewLine + player.Status;
            }

            if (gameController.GameOver == true)    //Если статус игры - Игра окончена
            {
                MessageBox.Show(gameController.WinnerStatus);                                                       //Вывести новым окном победителей в игре
                var choiseResult = MessageBox.Show("Хотите сыграть еще", "Игра окончена", MessageBoxButton.YesNo);  //Предложить новую игру
                if (choiseResult == MessageBoxResult.Yes)                                                           //Если пользователь согласена на новую игру
                {
                    gameController.NewGame();                                                                        //Начать новую игру с теми же данными
                    UpdateStatus();                                                                                  //Сбросить показатели игры
                }
                else
                    Close();                                                                                          //Либо закрыть игру
            }

            var sortedhand = gameController.HumanPlayer.Hand.OrderBy(card => card.Value).ToList();  //Отсортировать карты в руке игрока по масти
            HumanPlayerHand.ItemsSource = sortedhand;                                               //Отсортированные карты вывести на экран
            OpponentNamesList.ItemsSource = gameController.Opponents.ToList();                      //Вывести список противников на экран
        }

        /// <summary>
        /// Событие клик по кнопке начнёт новый раунд
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayRoundBottom_Click(object sender, RoutedEventArgs e)
        {
            PlayRound(); //Сыграть один раунд
        }
    }
}