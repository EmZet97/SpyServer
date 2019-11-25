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

namespace SpyServer
{
    /// <summary>
    /// Logika interakcji dla klasy ClientPanel.xaml
    /// </summary>
    public partial class ClientPanel : Page
    {
        Client client;
        public ClientPanel()
        {
            InitializeComponent();
            client = new Client("Klient", this);
            client.Start();
        }

        
    }
}
