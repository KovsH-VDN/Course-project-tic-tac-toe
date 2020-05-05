using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ClientXO
{
    /// <summary>
    /// Логика взаимодействия для Connect.xaml
    /// </summary>
    public partial class Connect : Window
    {
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public string Login { get; set; }
        public Connect()
        {
            InitializeComponent();
        }

        private void ConnectToServer(object sender, RoutedEventArgs e)
        {
            IPAddress = $"{textBoxIP1.Text}.{textBoxIP2.Text}.{textBoxIP3.Text}.{textBoxIP4.Text}";
            Port = int.Parse(textBoxPort.Text);
            Login = textBoxName.Text;
            DialogResult = true;
        }
    }
}
