using GalaSoft.MvvmLight.Command;
using SharedDataClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;
using System.Windows.Input;

namespace UNO
{
    public class LoginViewModel : INotifyPropertyChanged
    {

        readonly App App;
        User user;
        public ICommand LoginCommand { get; set; }


        public LoginViewModel(App app)
        {
            this.LoginCommand = new RelayCommand(() => { CanLogin(UserName); });
            this.App = app;
        }

        public string UserName { get; set; }

        private bool CanLogin(string userName)
        {
            bool output = false;

            if (!userName.Equals("System"))
            {
                output = true;

                user = new User(userName);

                Debug.WriteLine(UserName);

                LoginCheck();
            }

            return output;
        }

        private void LoginCheck()
        {
            //Send username and await ack. (((SERVER)))


            this.App.AfterSuccesfullLogin();
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
