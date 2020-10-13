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
using System.Collections;

namespace Chat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<User> userList;

        private string[] DefColors = { "black", "green", "red", "purple", "blue", "orange" };

        public MainWindow()
        {
            InitializeComponent();

            userList = new List<User>();

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
            StreamReader stream = QueryChatStatus();

            try
            {
                string value = stream.ReadToEnd();

                var stuff = ParseJson(value);

                PopulateUsers(stuff);

                PopulateUI(stuff);
            }
            finally
            {
                stream.Dispose();
            }
        }

        private void PopulateUsers(dynamic stuff)
        {
            var rnd = new Random();
            userList.Clear();
            foreach (var userName in stuff.users)
            {
                userList.Add(new User { Name = userName, FavoriteColor = DefColors[rnd.Next(DefColors.Length-1)] });
            }
        }

        private static StreamReader QueryChatStatus()
        {
            string url = "http://api.stepchat.site/";
            var req = HttpWebRequest.Create(url);
            var response = req.GetResponse();

            var stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            return stream;
        }

        private dynamic ParseJson(string value)
        {
            return JsonConvert.DeserializeObject(value);
        }

        private void PopulateUI(dynamic stuff)
        {
            ListBox.Items.Clear();
            foreach (var user in userList)
            {
                var item = new ListBoxItem();
                var nameBinding = new Binding("Name")
                {
                    Source = user
                };
                item.SetBinding(ContentProperty, nameBinding);

                var colorBinding = new Binding("FavoriteColor");
                colorBinding.Source = user;
                item.SetBinding(ForegroundProperty, colorBinding);

                ListBox.Items.Add(item);
            }

            MessagesView.Items.Clear();
            foreach(var message in stuff.messages)
            {
                var item = new ListViewItem();
                item.Content = message.name + " >> " + message.text ;                
                MessagesView.Items.Add(item);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReadChatStatus();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
