using System.Windows;

namespace UNO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NetworkCommunication NetworkCommunication;

        public MainWindow(App app, NetworkCommunication networkCommunication)
        {
            InitializeComponent();
            this.NetworkCommunication = networkCommunication;
            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel(app, networkCommunication);
            this.DataContext = mainWindowViewModel;
            
        }

    }
}
