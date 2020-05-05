using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientXO
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Client client;
        public MainWindow()
        {
            InitializeComponent();

            Connect connect = new Connect();
            if (connect.ShowDialog() == false)
                Close();

            else
            {
                client = new Client(
                    connect.IPAddress, 
                    connect.Port, 
                    connect.Login, 
                    message=>chat.Text+=message + "\r\n");
                
                DataContext = client;
            }
        }

        private async void MakeAMove(object sender, RoutedEventArgs e)
        {
            Button button = (Button)e.OriginalSource;
            int index = client.Cells.IndexOf((Cell)button.Tag);
            await client.SentGameData(2,index);
        }

        private async void Say(object sender, RoutedEventArgs e)
        {
            await client.SentMessage(1,message.Text);
            message.Text = "";
        }

        private void Show(object sender, TextChangedEventArgs e)
        {
            chat.ScrollToEnd();
        }
    }
}
