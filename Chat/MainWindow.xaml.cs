using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Threading;

namespace Chat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            dynamic timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan( 0, 0, 1 );
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            ReadChatStatus();
        }

        private void ReadChatStatus()
        {
            string url = "http://192.168.110.213:3000/";
            var req = HttpWebRequest.Create(url);
            var response = req.GetResponse();
            using (var stream = new StreamReader(
                response.GetResponseStream(), Encoding.UTF8
            ))
            {
                string value = stream.ReadToEnd();
                dynamic stuff = JsonConvert.DeserializeObject(value);
                ListBox.Items.Clear();
                foreach (var userName in stuff.users)
                {
                    var item = new ListBoxItem();
                    item.Content = userName;
                    ListBox.Items.Add(item);
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
