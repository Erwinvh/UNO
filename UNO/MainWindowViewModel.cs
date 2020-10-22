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

namespace UNO
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        readonly App app;
        public ICommand LaunchGameCommand { get; set; }
        public ICommand LeaveLobbyCommand { get; set; }
        private NetworkCommunication networkCommunication;
        public ObservableCollection<User> observableUsers { get; set; }

        public MainWindowViewModel(App app, NetworkCommunication networkCommunication)
        {
            
            this.networkCommunication = networkCommunication;
            this.LeaveLobbyCommand = new RelayCommand(() => { LeaveLobby(); });
            this.app = app;
            PlayerList = new ObservableCollection<User>();
            PlayerList.Add(new User("PlayerOne"));
            PlayerList.Add(new User("PlayerTwo"));
            PlayerList.Add(new User("PlayerThree"));
            PlayerList.Add(new User("PlayerFour"));
        }

        public void LeaveLobby()
        {
            networkCommunication.sendLobby(networkCommunication.user.name, "");

            app.LeaveLobby();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
