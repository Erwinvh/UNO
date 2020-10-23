using GalaSoft.MvvmLight.Command;
using SharedDataClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Automation;
using System.Windows.Input;
using SharedDataClasses;
using System.Diagnostics;

namespace UNO
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        readonly App app;
        public ICommand LaunchGameCommand { get; set; }
        public ICommand LeaveLobbyCommand { get; set; }
        public ICommand Addplayer { get; set; }
        private NetworkCommunication networkCommunication;
        public AsyncObservableCollection<User> observableUsers { get; set; }
        public List<Score> Scoreboard { get; set; }

        public MainWindowViewModel(App app, NetworkCommunication networkCommunication)
        {
            
            this.networkCommunication = networkCommunication;
            this.networkCommunication.mainWindowViewModel = this;
            this.LeaveLobbyCommand = new RelayCommand(() => { LeaveLobby(); });
            this.LaunchGameCommand = new RelayCommand(() => { LaunchGame(); });
            //this.Addplayer = new RelayCommand(() => { AddPlayer(string username); });
            this.app = app;
            Scoreboard = this.networkCommunication.Scoreboard;
            observableUsers = new AsyncObservableCollection<User>();
        }

        public void LeaveLobby()
        {
            networkCommunication.sendLobby(networkCommunication.user.name, "");

            app.LeaveLobby();
        }

        public void AddPlayer(string username)
        {
            foreach (User u in observableUsers) 
            {
                if (u.name.Equals(username))
                {
                    return;
                }
            }

            Debug.WriteLine("Added Player!!!");
            observableUsers.Add(new User(username));
        }

        public void RemovePlayer(string username)
        {
            foreach (User u in observableUsers)
            {
                if (u.name.Equals(username))
                {
                    observableUsers.Remove(u);
                }
            }
        }

        public void LaunchGame()
        {
            app.LaunchGame();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
