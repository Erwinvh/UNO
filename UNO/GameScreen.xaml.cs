using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for GameScreen.xaml
    /// </summary>
    public partial class GameScreen : Window
    {
        private NetworkCommunication NetworkCommunication;

        public GameScreen(App app, NetworkCommunication networkCommunication)
        {
            InitializeComponent();
            this.NetworkCommunication = networkCommunication;
            GameScreenViewModel gameScreenViewModel = new GameScreenViewModel(app, networkCommunication);
            this.DataContext = gameScreenViewModel;

        }
        public void window_Close(object sender, CancelEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
