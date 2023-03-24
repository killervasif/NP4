﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Policy;
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

namespace NP4
{
    
    public partial class MainWindow : Window
    {
        private UdpClient client;
        private IPEndPoint connectEP;
        
        public MainWindow()
        {
            InitializeComponent();
            client = new UdpClient();
            connectEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 45678);
        }

        private static BitmapImage? LoadImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var buffer=new byte[ushort.MaxValue-29];
            await client.SendAsync(buffer, buffer.Length, connectEP);
            var len = 0;
            var limit=buffer.Length;
            var list=new List<byte>();
            while (true)
            {
                do
                {
                    var result=await client.ReceiveAsync();
                    buffer=result.Buffer;
                    len = buffer.Length;
                    list.AddRange(buffer);
                } while(len==limit);
                Img.Source = LoadImage(list.ToArray());
                list.Clear();
            }
        }
    }
}
