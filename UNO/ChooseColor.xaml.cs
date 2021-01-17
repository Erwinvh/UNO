using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UNO
{
    /// <summary>
    /// Interaction logic for ChooseColor.xaml
    /// </summary>
    public partial class ChooseColor : Window
    {
        private GameScreenViewModel gameScreenViewModel {get; set;}

        public ChooseColor(GameScreenViewModel gameScreenViewModel)
        {
            InitializeComponent();
            this.gameScreenViewModel = gameScreenViewModel;
        }

        private void BLUE_Click(object sender, RoutedEventArgs e)
        {
            gameScreenViewModel.colorPicker = "blue";
            this.Close();
        }

        private void RED_Click(object sender, RoutedEventArgs e)
        {
            gameScreenViewModel.colorPicker = "red";
            this.Close();
        }

        private void YELLOW_Click(object sender, RoutedEventArgs e)
        {
            gameScreenViewModel.colorPicker = "yellow";
            this.Close();
        }

        private void GREEN_Click(object sender, RoutedEventArgs e)
        {
            gameScreenViewModel.colorPicker = "green";
            this.Close();
        }
    }
}
