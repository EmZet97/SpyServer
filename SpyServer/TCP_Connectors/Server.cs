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

        public Server(ServerPanel window)
        {
            mainWindow = window;
            WriteInfoAsync("Uruchomiono server");
            //Start thread listening for connecting clients
            Thread t = new Thread(ListenForClients);
            t.Start();
            WriteInfoAsync("Oczekuję klientów");
            Message first = new TextMessage();
            Messages.MessageReaderBuilder msgBuilder = new Messages.MessageReaderBuilder(first);
            msgBuilder.AddNext(new ImageMessage());
            msg_reader = msgBuilder.GetChain();
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
                        mainWindow.TextBlock.Text = obj.ToString();
                    });
                    
                }
                if(obj is Image)
                {
                    mainWindow.TextBlock.Dispatcher.Invoke(() =>
                    {
                        mainWindow.TextBlock.Text = obj.GetType().Name;
                    });
                    
                    Bitmap bmp = (Bitmap)obj;
                    Bitmap bmp2 = new Bitmap(bmp);
                    bmp2.Save("image" + ++lastImage + ".jpeg", ImageFormat.Jpeg);

                    //GC.KeepAlive()
                    mainWindow.ScreenShotImage.Dispatcher.Invoke(() =>
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri("image" + lastImage + ".jpeg", UriKind.Relative);
                        bitmap.EndInit();
                        mainWindow.ScreenShotImage.Source = bitmap;
                        //mainWindow.ScreenShotImage.Source = new BitmapImage(new Uri();// "Screenshots/image.png";
                        //bmp.Save("screenshot.bmp");
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
