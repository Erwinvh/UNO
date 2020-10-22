using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace UNO
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        readonly App app;
        public ICommand LaunchGameCommand { get; set; }
        public ICommand LeaveLobbyCommand { get; set; }
        private NetworkCommunication networkCommunication;

        public MainWindowViewModel(App app, NetworkCommunication networkCommunication)
        {
            this.networkCommunication = networkCommunication;
            this.LeaveLobbyCommand = new RelayCommand(() => { LeaveLobby(); });
            this.app = app;
        }

        public void LeaveLobby()
        {
            networkCommunication.sendLobby(networkCommunication.user.name, "");

            app.LeaveLobby();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
