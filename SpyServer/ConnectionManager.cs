using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SpyServer
{
    class ConnectionManager
    {
        private bool activeConnection = false;
        private TcpListener server = null;
        private TcpClient client = null;

        private BinaryReader reader = null;
        private BinaryWriter writer = null;


        public void StartServer()
        {
            string IP = "127.0.0.1";
            int port = 1234;
            IPAddress serwerIP;
            
            try
            {
                serwerIP = IPAddress.Parse(IP);
                // MessageBox.Show(adresIP);
            }
            catch
            {
                //MessageBox.Show("Błędny adres IP");
                activeConnection = false;
                return;
            }

            server = new TcpListener(serwerIP, (int)port);
            try
            {
                server.Start();
                //SetText("Oczekiwanie na połączenie...");
                client = server.AcceptTcpClient();
                NetworkStream ns = client.GetStream();
                //SetText("Klient próbuje się połączyć");
                reader = new BinaryReader(ns);
                writer = new BinaryWriter(ns);
                if (reader.ReadString() == "###HI###")
                {
                    //SetText("Połączono :)");
                    //backgroundWorker2.RunWorkerAsync();
                    //MessageBox.Show(textBox2.Text);
                    writer.Write("Hello there");
                }
                else
                {
                    //SetText("Klient nie wykonał wymaganej autoryzacji. Połączenie przerwane");
                    client.Close();
                    server.Stop();
                    activeConnection = false;
                }
            }
            catch
            {
                //SetText("Połączenie przerwane :(");

                activeConnection = false;
            }
        }
    }
}
