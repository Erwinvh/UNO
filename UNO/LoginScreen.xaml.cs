using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UNO
{
    /// <summary>
    /// Interaction logic for LoginScreen.xaml
    /// </summary>
    public partial class LoginScreen : Window
    {
        private NetworkCommunication NetworkCommunication;

        public LoginScreen(App app, NetworkCommunication networkCommunication)
        {
            this.NetworkCommunication = networkCommunication;
            LoginViewModel loginViewModel = new LoginViewModel(app, networkCommunication);
            this.DataContext = loginViewModel;
            InitializeComponent();
        }
        public void window_Close(object sender, CancelEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
