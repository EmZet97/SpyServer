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
    class Server
    {
        private const int port = 5000;

        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();
        private ServerPanel mainWindow;

        public Server(ServerPanel window)
        {
            mainWindow = window;
            WriteInfoAsync("Uruchomiono server");
            //Start thread listening for connecting clients
            Thread t = new Thread(ListenForClients);
            t.Start();
            WriteInfoAsync("Oczekuję klientów");
        }

        private void ListenForClients()
        {
            int count = 1;
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, port);

            ServerSocket.Start();
            while (true)
            {
                TcpClient client = ServerSocket.AcceptTcpClient();
                lock (_lock) list_clients.Add(count, client);

                //Start thread listening for messages from clients
                Thread ct = new Thread(new ParameterizedThreadStart(o => ReceiveData((TcpClient)o)));
                ct.Start(client);
                WriteInfoAsync("Połączono z nowym klientem");

                count++;
            }
        }

        /*No jobs for that function
        public void HandleClients(object o)
        {
            int id = (int)o;
            TcpClient client;

            lock (_lock) client = list_clients[id];

            while (true)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int byte_count = stream.Read(buffer, 0, buffer.Length);

                if (byte_count == 0)
                {
                    break;
                }

                string data = Encoding.ASCII.GetString(buffer, 0, byte_count);
                Broadcast(data);
                WriteInfoAsync(data);
            }

            lock (_lock) list_clients.Remove(id);
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        */

        //Get data from client
        public void ReceiveData(TcpClient client)
        {
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = client.GetStream().Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                WriteInfoAsync("- Server reading:  <<" + Encoding.ASCII.GetString(receivedBytes, 0, byte_count) + ">>");
            }
        }

        //Send data to all clients
        public void Broadcast(string data)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            lock (_lock)
            {
                foreach (TcpClient c in list_clients.Values)
                {
                    NetworkStream stream = c.GetStream();

                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        //Write logs on GUI panel
        private void WriteInfo(string text)
        {
            mainWindow.LogTextBox.Text += text;
        }

        //Write logs on GUI panel async
        private void WriteInfoAsync(string text)
        {
            mainWindow.LogTextBox.Dispatcher.Invoke(() =>
            {
                mainWindow.LogTextBox.Text += text + "\n";
            });
        }

    }
}
