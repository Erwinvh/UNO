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

        public MainWindowViewModel(App app)
        {
            this.LeaveLobbyCommand = new RelayCommand(() => { LeaveLobby(); });
            this.app = app;
        }

        public void LeaveLobby()
        {
            app.LeaveLobby();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
