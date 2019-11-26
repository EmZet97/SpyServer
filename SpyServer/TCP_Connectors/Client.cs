using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SpyServer
{
    class Client
    {
        private readonly string name;
        private readonly static IPAddress serverIP = IPAddress.Parse("127.0.0.1");
        private const int port = 5000;

        private NetworkStream ns;
        private readonly ClientPanel mainWindow;

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
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(serverIP, port);
                ns = client.GetStream();
                WriteInfoAsync("Połączono z serwerem");

                //Start new thread receiving messages from server
                Thread thread = new Thread(o => ReceiveData());
                thread.Start(client);
            }
            catch (System.Net.Sockets.SocketException)
            {
                WriteInfoAsync("Host odmawia połączenia");
                //ConnectClient();
            }
            
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
                string data = Encoding.ASCII.GetString(receivedBytes, 0, byte_count);
                WriteInfoAsync("-" + name + " czyta:  <<" + data + ">>");
                switch (data)
                {
                    case "2":
                        WriteInfoAsync("Serwer rzada tekstu");
                        SendText(RandomString(12, false));
                        break;
                    case "3":
                        WriteInfoAsync("Serwer rzada zrzutu ekranu");
                        byte[] image = ClientManager.BitmapSourceToByte(ClientManager.CopyScreen());
                        SendImage(image);
                        break;
                }

            }
        }

        public void SendText(string text)
        {
            string data = text;

            byte[] msgCode = { byte.Parse("2") };
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            byte[] combined = msgCode.Concat(buffer).ToArray();
            ns.Write(combined, 0, combined.Length);
        }
        public void SendImage(byte[] image)
        {
           
            byte[] msgCode = { byte.Parse("3") };
            byte[] buffer = image;

            byte[] combined = msgCode.Concat(buffer).ToArray();
            ns.Write(combined, 0, combined.Length);
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
        // Generate a random string with a given size  
        public string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

    }


}
