using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GoFishUI
{
    /// <summary>
    /// Логика взаимодействия для PlayerNameWindow.xaml
    /// </summary>
    public partial class PlayerNameWindow : Window
    {
        public string PlayerName { get; private set; }  //Имя игрока человека
        public int OpponentCount {  get; private set; } //Количесиво компьютерных игроков
        
        public PlayerNameWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Событие, пользователь кликнул на кнопку окей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            //Если пользователь не ввёл имя, присвоить унизительно имя, либо использовать введённое пользователем имя
            PlayerName = PlayerNameTextBox.Text == "" ? "Пердун" : PlayerNameTextBox.Text;
            //Количество компьютерных игроков определяеться значением ползунка
            OpponentCount = (int)OpponentCountSlider.Value;
            DialogResult = true;
        }
    }
}
