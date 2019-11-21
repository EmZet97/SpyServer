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
        private NetworkStream ns;
        private MainWindow mainWindow;
        private TcpClient server;

        public Client(string name)
        {
            this.name = name;
        }
        public void StartClient(MainWindow window)
        {
            mainWindow = window;
            Thread thread = new Thread(ConnectClient);
            thread.Start();
        }
        private void ConnectClient()
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 5000;
            TcpClient client = new TcpClient();
            server = client;
            client.Connect(ip, port);
            //WriteInfoAsync("client connected!!");
            ns = client.GetStream();
            
            Thread thread = new Thread(o => ReceiveData());

            thread.Start(client);
            /*
            string s;
            while (!string.IsNullOrEmpty((s = Console.ReadLine())))
            {
                byte[] buffer = Encoding.ASCII.GetBytes(s);
                ns.Write(buffer, 0, buffer.Length);
            }

            client.Client.Shutdown(SocketShutdown.Send);
            thread.Join();
            ns.Close();
            client.Close();
            WriteInfoAsync("disconnect from server!!");
            //Console.ReadKey();*/
        }
        public void Disconnect()
        {

        }

        private void WriteInfo(string text)
        {
            mainWindow.INFO.Text += text;
        }
        private void WriteInfoAsync(string text)
        {
            mainWindow.INFO.Dispatcher.Invoke(() =>
            {
                mainWindow.INFO.Text += text;
            });
        }

        public void ReceiveData()
        {
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                WriteInfoAsync("-" + name + " reading:  <<" + Encoding.ASCII.GetString(receivedBytes, 0, byte_count) + ">>\n");
                
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
            //BinaryWriter writer = new BinaryWriter(ns);
            ns.Write(buffer, 0, buffer.Length);
            //ns.Close();
            //ns = server.GetStream();
            //ns.Flush();
            
            //writer.Write("Hello there, it's " + name);
            //writer.Close();
        }
    }


}
