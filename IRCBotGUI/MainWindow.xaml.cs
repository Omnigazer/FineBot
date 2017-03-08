using System;
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
using IRCBot;
using System.Threading;
using System.Windows.Threading;
using System.Net;

namespace IRCBotGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        public IRCClient client;        
        // Stub
        public Dictionary<string, TabItem> tabs;
        public Dictionary<TabItem, string> channels;
        // Config
        public Color TextColor { get; set; }
        public Color ActionColor { get; set; }        
        //

        public TabItem CurrentTab
        {
            get
            {
                return (TabItem)channelTabs.SelectedItem;
            }
        }

        public string CurrentChannel
        {
            get
            {
                return channels[CurrentTab];
            }
        }
        #endregion
        #region Init
        public MainWindow()
        {
            InitializeComponent();
            InitializeGUI();
            InitializeBot();
            Application.Current.Exit += Current_Exit;
        }

        public void InitializeGUI()
        {           
            tabs = new Dictionary<string,TabItem>();
            channels = new Dictionary<TabItem, string>();
            ActionColor = Color.FromRgb(0, 255, 0);
            TextColor = Color.FromRgb(0, 0, 0);
        }

        public void InitializeBot()
        {            
            // var addr = Dns.GetHostEntry("irc.livedubna.ru");
            var addr = Dns.GetHostEntry("irc.quakenet.org");            
            client = new IRCClient(addr.AddressList.First(), 6667);
            Bot bot = new Bot(client);
            client.NewMessage += new EventHandler<IRCClient.StringArgs>(ShowServerMessage);
            client.NewPrivateMessage += new EventHandler<IRCClient.MessageArgs>(ShowMessage);
            client.NewAction += new EventHandler<IRCClient.MessageArgs>(ShowAction);
            client.JoinedChannel += JoinedChannel;
            client.LeftChannel += LeftChannel;
            client.ConnectionRegistered += delegate { client.Join("#testchanx"); };
            client.Start();
            // a status window with hardcoded logic
            OpenChannel("Status");            
        }
        #endregion
        #region Methods
        public void OpenChannel(string channel)
        {
            TabItem tab = new TabItem();
            tab.Width = 100;
            tab.Height = 20;
            channelTabs.Items.Add(tab);
            ChannelTab tab_content = new ChannelTab();
            tab.Content = tab_content;            
            tab.Header = channel;
            tabs.Add(channel, tab);
            channels.Add(tab, channel);
            channelTabs.SelectedItem = tab;
        }

        public void CloseChannel(string channel)
        {
            if (tabs.ContainsKey(channel) && channels.ContainsKey(tabs[channel]))
            {
                TabItem tab = tabs[channel];
                tabs.Remove(channel);
                channels.Remove(tab);
                channelTabs.Items.Remove(tab);
                channelTabs.SelectedItem = channelTabs.Items[channelTabs.Items.Count - 1];
            }
            else
            {
                throw new Exception("Tab/channel not present in application");
            }
        }               

        /// <summary>
        /// Send an IRC command to the server.
        /// </summary>        
        public void InputCommand(string str)
        {
            if (str.StartsWith("/me "))
            {                
                if (str.Length > 4)
                {
                    string action = str.Substring(4);
                    client.SendAction(action, CurrentChannel);                    
                }
            }
            else if (str.StartsWith("/join "))
            {
                if (str.Length > 6)
                {
                    string channel = str.Substring(6);
                    client.Join(channel);                    
                }
            }
            else if (str.StartsWith("/part "))
            {
                if (str.Length > 6)
                {
                    string channel = str.Substring(6);
                    client.Part(channel);                    
                }
            }
            else if (str.StartsWith("/raw "))
            {
                if (str.Length > 5)
                {
                    string command = str.Substring(5);
                    client.IssueCommand(command);
                }
            }
            else
            {
                client.SendMessage(str, CurrentChannel);
                ListBox outputBox = ((ChannelTab)CurrentTab.Content).ChatWindow;
            }
        }
        #endregion
        #region Event Handlers
        // Triggers on Application.Exit()
        void Current_Exit(object sender, ExitEventArgs e)
        {
            client.Quit();
        }
        // Triggers when someone leaves a channel
        void LeftChannel(object sender, IRCClient.JoinArgs e)
        {
            if (e.Nick == client.CurrentNick)
            {
                CloseChannel(e.Channel);
            }
            else
            {
                // update user list
            }
        }
        // Triggers when someone joins a channel
        void JoinedChannel(object sender, IRCClient.JoinArgs e)
        {
            if (e.Nick == client.CurrentNick)
            {
                OpenChannel(e.Channel);
            }
            else
            {
                // update user list
            }
        }
        // Triggers when someone says something
        public void ShowMessage(object sender, IRCClient.MessageArgs e)
        {
            string str = String.Format("<{0}> {1}", e.Sender, e.Message);
            //outputBox.Items.Add(str);
            if (tabs.ContainsKey(e.Target))
            {
                //tabs[e.Target].Items.Add(str);
                ListBox outputBox = ((ChannelTab)tabs[e.Target].Content).ChatWindow;
                var item = new ListBoxItem();
                item.Content = str;
                item.Foreground = new SolidColorBrush(TextColor);
                outputBox.Items.Add(item);
            }
            else
            {

                // a private tab, really
                if (!tabs.ContainsKey(e.Sender))
                {
                    OpenChannel(e.Sender);
                }
                ListBox outputBox = ((ChannelTab)tabs[e.Sender].Content).ChatWindow;
                var item = new ListBoxItem();
                item.Content = str;
                item.Foreground = new SolidColorBrush(TextColor);
                outputBox.Items.Add(item);
            }
        }
        // Triggers when someone says "/me foo"
        public void ShowAction(object sender, IRCClient.MessageArgs e)
        {
            string str = String.Format("{0} {1}", e.Sender, e.Message);
            //outputBox.Items.Add(str);
            if (tabs.ContainsKey(e.Target))
            {
                //tabs[e.Target].Items.Add(str);
                ListBox outputBox = ((ChannelTab)tabs[e.Target].Content).ChatWindow;
                //outputBox.Items.Add(str);
                var item = new ListBoxItem();
                item.Content = str;
                item.Foreground = new SolidColorBrush(ActionColor);
                outputBox.Items.Add(item);
            }
        }
        // Triggers whenever you get a message from server.
        public void ShowServerMessage(object sender, IRCClient.StringArgs e)
        {
            // redundant проверка
            if (tabs.ContainsKey("Status"))
            {
                //tabs[e.Target].Items.Add(str);
                ListBox outputBox = ((ChannelTab)tabs["Status"].Content).ChatWindow;
                outputBox.Items.Add(e.Message);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string message = inputBox.Text;
                inputBox.Text = "";                          
                TabItem currentTab = (TabItem)channelTabs.SelectedItem; 
                InputCommand(message);                
            }
        }
        #endregion
    }
}
