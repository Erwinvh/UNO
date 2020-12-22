using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
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
            Debug.WriteLine("logincheck:" + NetworkCommunication.isLobbyReady);
            while (NetworkCommunication.isLobbyReady == null)
            {
                
                Thread.Sleep(100);
                Debug.WriteLine("logincheck loop:" + NetworkCommunication.isLobbyReady);
            }
            Debug.WriteLine("logincheck:" + NetworkCommunication.isLobbyReady);
            if (NetworkCommunication.isLobbyReady ?? true)
            {
                Debug.WriteLine("Main lobby opened");
               // main = new MainWindow(this, NetworkCommunication);
                main.Show();
                loginScreen.Hide();
            }
        }

        public void LeaveLobby()
        {
            loginScreen.Show();

            main.Hide();
        }

        public void LaunchGame()
        {
            
            gameScreen.Show();
            main.Hide();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            NetworkCommunication.disconnect();
        }

        internal void ReturnToLogin()
        {
            gameScreen.Hide();
            loginScreen.Show();
        }

        internal void ReturnToLobby()
        {
            gameScreen.Hide();
                main.Show();
        }

    }
}
