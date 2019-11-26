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
    /// Logika interakcji dla klasy ServerPanel.xaml
    /// </summary>
    public partial class ServerPanel : Page
    {
        Server server;
        public ServerPanel()
        {
            InitializeComponent();
            server = new Server(this);
        }

        private void SsButton_Click(object sender, RoutedEventArgs e)
        {
            server.Broadcast("2");
        }

        private void KlRadioButton_Click(object sender, RoutedEventArgs e)
        {
            server.Broadcast("3");
        }

        ~ServerPanel()
        {
            server.Stop();
        }

        
    }
}
