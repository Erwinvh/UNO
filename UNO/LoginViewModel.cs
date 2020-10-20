using ClientGUI.Commands;
using GalaSoft.MvvmLight.Command;
using SharedDataClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Input;
using RelayCommand = ClientGUI.Commands.RelayCommand;

namespace UNO
{
    public class LoginViewModel : INotifyPropertyChanged
    {

        readonly App App;
        User user;

        public LoginViewModel(App app)
        {
            this.App = app;
        }

        public string UserName { get; set; } = "";

        private bool CanLogin(string userName)
        {
            bool output = false;

            if (!userName.Equals("System"))
            {
                output = true;

                user = new User(userName);

                Console.WriteLine(user);

                this.App.AfterSuccesfullLogin();
            }

            return output;
        }

        private void LoginCheck()
        {
            //Send username and await ack. (((SERVER)))

            

            this.App.AfterSuccesfullLogin();
        }

        private ICommand mLoginCommand;
        public ICommand LoginCommand
        {
            get
            {
                if (mLoginCommand == null)
                {
                    mLoginCommand = new RelayCommand(param => CanLogin(UserName), param => true);
                }
                return mLoginCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
