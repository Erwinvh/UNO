using GalaSoft.MvvmLight.Command;
using SharedDataClasses;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace UNO
{
    public class LoginViewModel : INotifyPropertyChanged
    {

        readonly App App;
        User user;
        public ICommand LoginCommand { get; set; }
        private NetworkCommunication networkCommunication;


        public LoginViewModel(App app, NetworkCommunication NetworkCommunication)
        {
            this.networkCommunication = NetworkCommunication;
            this.LoginCommand = new RelayCommand(() => { CanLogin(UserName, LobbyCode); });
            this.App = app;
        }

        public string UserName { get; set; }
        public string LobbyCode { get; set; }

        private bool CanLogin(string userName, string lobbycode)
        {
            bool output = false;

            if (userName == null || lobbycode == null)
            {
                return output;
            }

            userName = RemoveWhitespace(userName);
            lobbycode = RemoveWhitespace(lobbycode);

            if (!userName.Equals("System") && !userName.Equals("") && !lobbycode.Equals(""))
            {
                output = true;

                user = new User(userName);

                Debug.WriteLine(UserName);

                Login();
            }

            return output;
        }

        private void Login()
        {
            //Send username and await ack. (((SERVER)))
            networkCommunication.sendLobby(UserName, LobbyCode);

            App.AfterSuccesfullLogin();
        }

        public string RemoveWhitespace(string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
