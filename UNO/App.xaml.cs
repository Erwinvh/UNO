using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace UNO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private LoginScreen loginScreen;
        private MainWindow main;
        private GameScreen gameScreen;
        private NetworkCommunication NetworkCommunication;

        protected override void OnStartup(StartupEventArgs e)
        {
            
            this.NetworkCommunication = new NetworkCommunication("localhost", 15243);
            this.NetworkCommunication.app = this;
            main = new MainWindow(this, NetworkCommunication);
            loginScreen = new LoginScreen(this, NetworkCommunication);
            gameScreen = new GameScreen(this, NetworkCommunication);
            loginScreen.Show();

        }

        public async Task AfterSuccesfullLogin()
        {
            while (NetworkCommunication.isLobbyReady == null)
            {
                
                Thread.Sleep(100);
            }

            if (NetworkCommunication.isLobbyReady ?? true)
            {
                main.Show();

                loginScreen.Close();
            }
        }

        public void LeaveLobby()
        {
            loginScreen = new LoginScreen(this, NetworkCommunication);
            loginScreen.Show();

            main.Close();
        }

        public void LaunchGame()
        {
            
            gameScreen.Show();
            main.Close();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            NetworkCommunication.disconnect();
        }
    }
}
