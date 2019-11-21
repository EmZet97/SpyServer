using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace SpyServer
{
    class Client
    {
        private string name;
        private static IPAddress serverIP = IPAddress.Parse("127.0.0.1");
        private const int port = 5000;

        private NetworkStream ns;
        private ClientPanel mainWindow;

        public Client(string name, ClientPanel window)
        {
            this.name = name;
            mainWindow = window;

        }
        public void Start()
        {
            WriteInfoAsync("Uruchamiam połączenie");
            //Start thread connecting client to server
            Thread thread = new Thread(ConnectClient);
            thread.Start();
        }
        private void ConnectClient()
        {            
            TcpClient client = new TcpClient();
            client.Connect(serverIP, port);
            ns = client.GetStream();
            WriteInfoAsync("Połączono z serwerem");

            //Start new thread receiving messages from server
            Thread thread = new Thread(o => ReceiveData());
            thread.Start(client);
        }
        public void Disconnect()
        {

        }        

        public void ReceiveData()
        {
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                WriteInfoAsync("-" + name + " reading:  <<" + Encoding.ASCII.GetString(receivedBytes, 0, byte_count) + ">>");
                
            }
        }

        public void SendData(string text)
        {
            string data;
            if (text == null)
                data = "";
            else
                data = text + name;
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            ns.Write(buffer, 0, buffer.Length);
        }

        private void WriteInfo(string text)
        {
            mainWindow.LogTextBox.Text += text;
        }
        private void WriteInfoAsync(string text)
        {
            mainWindow.LogTextBox.Dispatcher.Invoke(() =>
            {
                mainWindow.LogTextBox.Text += text + "\n";
            });
        }   
    }


}
