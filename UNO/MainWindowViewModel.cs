﻿using GalaSoft.MvvmLight.Command;
using SharedDataClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;



namespace UNO
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        
        readonly App app;
        public ICommand ReadyPlayerCommand { get; set; }
        public ICommand LeaveLobbyCommand { get; set; }
        //public ICommand Addplayer { get; set; }
        private NetworkCommunication networkCommunication;
        public AsyncObservableCollection<User> observableUsers { get; set; }
        public AsyncObservableCollection<Score> Scoreboard { get; set; }
        public string lobbyCode { get; set; }

        public MainWindowViewModel(App app, NetworkCommunication networkCommunication)
        {
            
            this.networkCommunication = networkCommunication;
            this.networkCommunication.mainWindowViewModel = this;
            this.LeaveLobbyCommand = new RelayCommand(() => { LeaveLobby(); });
            this.ReadyPlayerCommand = new RelayCommand(() => { sendReadyMessage(); });
            //this.Addplayer = new RelayCommand(() => { AddPlayer(string username); });
            this.app = app;
            Scoreboard = new AsyncObservableCollection<Score>();
            observableUsers = new AsyncObservableCollection<User>();
        }

        //
        //--resets the ready status of every player--
        //
        public void resetReady()
        {
            foreach(User player in observableUsers)
            {
                player.isReady = false;
            }
        }

        //
        //--starts the proccess to leave the lobby--
        //
        public void LeaveLobby()
        {
            networkCommunication.sendLobby(networkCommunication.user.name, "");

            app.LeaveLobby();
        }

        public void sendReadyMessage()
        {
            networkCommunication.sendToggleReady();
        }

        public void setLobbyCode(string lobbyCode)
        {
            this.lobbyCode = "UNO Lobby: " + lobbyCode;
        }

        //
        //--readies a player by name and checks if everybody is ready--
        //
        public void readyPlayer(string username)
        {
            foreach (User player in observableUsers)
            {
                if (player.name == username)
                {
                    player.isReady = !player.isReady;
                    Debug.WriteLine(player.name + " This players info was changed to " + player.isReady);
                }
            }

            if (observableUsers.Count < 2)
            {
                return;
            }
            foreach (User user in observableUsers)
            {
                if (!user.isReady)
                {
                    return;
                }
            }
            LaunchGame();
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
                    return;
                }
            }
        }

        public void LaunchGame()
        {
            if (!app.Dispatcher.CheckAccess())
            {
                networkCommunication.GameScreenViewModel.transportPlayers(observableUsers);
                app.Dispatcher.InvokeAsync(new Action(app.LaunchGame));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void emptyObservableUsers()
        {
            observableUsers = new AsyncObservableCollection<User>();
        }

        public void updateScoreboard(List<Score> updatedScoreboard)
        {
            Scoreboard.Clear();
            foreach (Score score in updatedScoreboard)
            {
                Scoreboard.Add(score);
            }
        }
    }
}
