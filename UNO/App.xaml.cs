using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace UNO
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private LoginScreen loginScreen;
        private MainWindow main;

        protected override void OnStartup(StartupEventArgs e)
        {
            loginScreen = new LoginScreen(this);
            loginScreen.Show();

            //base.OnStartup(e);
            // MainWindow mainWindow = new MainWindow();
            // ViewModel viewModel = new ViewModel();
            //// ClientApp client = new ClientApp();

            // mainWindow.DataContext = viewModel;
            // mainWindow.Show();
        }

        public void AfterSuccesfullLogin()
        {
            main = new MainWindow();
            main.Show();

            loginScreen.Close();
        }
    }
}
