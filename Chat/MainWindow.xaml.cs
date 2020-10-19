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
using System.Threading;

namespace Chat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<User> userList;
        private List<Message> messageList;
        dynamic stuff;

        private string[] DefColors = { "black", "green", "red", "purple", "blue", "orange" };
        private string[] DefIcons = {
                "https://upload.wikimedia.org/wikipedia/commons/3/38/Wikipedia_User-ICON_byNightsight.png",
                "https://upload.wikimedia.org/wikipedia/commons/5/50/Farm-Fresh_user_king.png",
                "https://live.staticflickr.com/39/83249642_dfe7d7aa53_b.jpg"
            };

        public MainWindow()
        {
            InitializeComponent();

            userList = new List<User>();
            messageList = new List<Message>();

            //dynamic timer = new DispatcherTimer();
            //timer.Tick += new EventHandler(timer_Tick);
            //timer.Interval = new TimeSpan( 0, 0, 1 );
            //timer.Start();
            var timer = new Timer(OnTimer, null, 0, 1000);
        }

        private void OnTimer(object state)
        {
            ReadChatStatus();
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

                stuff = ParseJson(value);

                PopulateUsers(stuff);

                messageList.Clear();
                foreach (var  msg in stuff.messages)
                {
                    var newMessage = new Message();
                    newMessage.Text = msg.text;

                    var newUser = from user in userList where user.Name == msg.name.ToString() select user;

                    newMessage.User = userList.Find(u => u.Name == msg.name.ToString());
                    messageList.Add(newMessage);
                }

                ListBox.Dispatcher.Invoke(PopulateUIDefault);                
            }
            finally
            {
                stream.Dispose();
            }
        }

        private void PopulateUIDefault()
        {
            PopulateUI(stuff);
        }

        private void PopulateUsers(dynamic stuff)
        {
            var rnd = new Random();
            userList.Clear();
            foreach (var userName in stuff.users)
            {
                userList.Add(new User {
                    Name = userName,
                    FavoriteColor = DefColors[rnd.Next(DefColors.Length)],
                    ImageUrl = DefIcons[rnd.Next(DefIcons.Length)] });
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
            ListBox.ItemsSource = userList;
            MessagesView.ItemsSource = messageList;
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

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private bool SendMessage()
        {
            string url = "http://api.stepchat.site/message";
            // name
            // text
            var request = (HttpWebRequest)HttpWebRequest.Create(url);

            var postData = "{\"name\":\"test\",";
            postData += $"\"text\":\"{txtMessage.Text}\"}}";
            var data = Encoding.ASCII.GetBytes(postData);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
            
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseString = reader.ReadToEnd();
                    MessageBox.Show(responseString);
                }

                return response.StatusCode == HttpStatusCode.OK;
            }
            catch(WebException we)
            {
                MessageBox.Show(we.Message);
                return false;
            }
        }
    }
}
