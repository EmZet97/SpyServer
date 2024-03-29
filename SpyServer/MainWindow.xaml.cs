﻿using System;
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
        public static Frame frame;
        public MainWindow()
        {
            InitializeComponent();
            frame = PageFrame;
            MainWindow.NavigateFrame(new StartPanel());
        }

        public static void NavigateFrame(object panel)
        {
            frame.Navigate(panel);
        }

     
    }
}
