using GalaSoft.MvvmLight.Command;
using SharedDataClasses;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace UNO
{
    public class LoginViewModel : INotifyPropertyChanged
    {

        readonly App App;
        User user;
        public ICommand LoginCommand { get; set; }
        private NetworkCommunication networkCommunication;
        public bool isOpen = false;
        

        public LoginViewModel(App app, NetworkCommunication NetworkCommunication)
        {
            this.networkCommunication = NetworkCommunication;
            this.networkCommunication.LoginViewModel = this;
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
                MakeMessageBox("Name = System or\n" +
                                "Name is empty or\n" +
                                "Lobbycode is empty \n");
                return output;
            }

            userName = RemoveWhitespace(userName);
            lobbycode = RemoveWhitespace(lobbycode);

            if (!userName.Equals("System") && !userName.Equals("system") &&!userName.Equals("") && !lobbycode.Equals("") && !userName.Equals("You") && !userName.Equals("you"))
            {
                output = true;

                user = new User(userName);

                Debug.WriteLine(UserName);

                LoginAsync();
            } else
            {
                MakeMessageBox("Name = System or\n" +
                                "Name is empty or\n" +
                                "Lobbycode is empty \n");
            }

            return output;
        }

        public void MakeMessageBox(string message)
        {
            MessageBox.Show(message);
        }

        private async Task LoginAsync()
        {
            //Send username and await ack. (((SERVER)))
            networkCommunication.sendLobby(UserName, LobbyCode);
            await networkCommunication.untilLobbyReadyAsync();
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
