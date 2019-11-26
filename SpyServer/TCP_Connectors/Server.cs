using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;

namespace SpyServer
{
    class Server
    {
        private const int port = 5000;

        static readonly object _lock = new object();
        static readonly Dictionary<int, TcpClient> list_clients = new Dictionary<int, TcpClient>();
        private ServerPanel mainWindow;
        Message msg_reader = null;
        private static int lastImage = 0;

        private readonly Thread clientsListenerThread;
        private List<Thread> clientsReaderThread;

        public Server(ServerPanel window)
        {
            mainWindow = window;

            WriteInfoAsync("Uruchomiono server");

            //Start thread listening for connecting clients
            clientsListenerThread = new Thread(ListenForClients);
            clientsListenerThread.Start();
            clientsReaderThread = new List<Thread>();

            WriteInfoAsync("Oczekuję klientów");

            //Build message reader chain
            Message first = new TextMessage();
            Messages.MessageReaderBuilder msgBuilder = new Messages.MessageReaderBuilder(first);
            msgBuilder.AddNext(new ImageMessage());
            msg_reader = msgBuilder.GetChain();
        }

        ~Server()
        {
            Stop();
        }

        public void Stop()
        {
            clientsListenerThread.Abort();
            foreach (Thread clientReader in clientsReaderThread)
            {
                clientReader.Abort();
            }
            Console.WriteLine("Threads deleted");
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
                clientsReaderThread.Add(ct);

                WriteInfoAsync("Połączono z nowym klientem");

                count++;
            }
        }

        //Get data from client
        public void ReceiveData(TcpClient client)
        {
            byte[] receivedBytes = new byte[10000000];
            int byte_count;

            while ((byte_count = client.GetStream().Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                //WriteInfoAsync("- Server reading:  <<" + Encoding.ASCII.GetString(receivedBytes, 0, byte_count) + ">>");
                ConvertData(receivedBytes);
            }
        }

        private void ConvertData(byte[] data)
        {
            msg_reader.Content = data;
            Object obj = msg_reader.GetSpecyficObject();
            if(obj != null)
            {
                if(obj is string)
                {
                    mainWindow.TextBlock.Dispatcher.Invoke(() =>
                    {
                        mainWindow.TextBlock.Text += "Otrzymano tekst   ====>\n";
                    });
                    mainWindow.TextBlock2.Dispatcher.Invoke(() =>
                    {
                        mainWindow.TextBlock2.Text += obj.ToString() + "\n";
                    });

                }
                if(obj is Image)
                {
                    mainWindow.TextBlock.Dispatcher.Invoke(() =>
                    {
                        mainWindow.TextBlock.Text += "Otrzymano zrzut ekranu\n";
                    });
                    
                    Bitmap bmp = (Bitmap)obj;
                    Bitmap bmp2 = new Bitmap(bmp);
                    string name = "image" + ++lastImage + ".jpeg";
                    bmp2.Save(name, ImageFormat.Jpeg);
                    
                    mainWindow.TextBlock2.Dispatcher.Invoke(() =>
                    {
                        mainWindow.TextBlock2.Text += "Zrzut ekranu zapisany jako: " + name + "\n";
                    });
                    
                }
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
