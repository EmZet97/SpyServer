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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpyServer
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static int clients = 0;
        private Server server;
        private Client client;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (server != null)
                return;
            server = new Server();
            server.StartServer(this);
            
        }

        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            Client newclient = new Client("client" + clients++) ;
            newclient.StartClient(this);
            if (client == null)
                client = newclient;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (server != null)
                server.Broadcast("Witam");

        }

        private void ClientSendButton_Click(object sender, RoutedEventArgs e)
        {
            client.SendData("Hello there, it's ");
            client.SendData(null);
        }
    }
}
