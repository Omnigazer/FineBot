using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Configuration;

namespace IRCBot
{
    class Program
    {
        static void Main(string[] args)
        {           
            /*
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());
            var addr = Dns.GetHostEntry("irc.livedubna.ru");            
            //var addr = Dns.GetHostEntry("keeper.wtf");            
            IRCClient client = new IRCClient(addr.AddressList.First(), 6667);
            Bot bot = new Bot(client);
            SendOrPostCallback d = new SendOrPostCallback(client.InputCommand);            
            client.Start();
            //var x = ConfigurationSettings.AppSettings.Get("sample");
            new Thread(() =>
            {
                string command;
                Thread.CurrentThread.IsBackground = true;
                
                while (true)
                {
                    command = Console.ReadLine();
                    d.Invoke(command);
                }
            }).Start();
            Dispatcher.Run();                       
            */

            /*
            MockClient mock_client = new MockClient();
            Bot bot = new Bot(mock_client);            
            while (true)
            {
                string command = Console.ReadLine();
                if (command == "exit") break;
                //bot.HandleCommand(String.Format("hui PRIVMSG #pokerland :{0}", command));  
                mock_client.HandleCommand(String.Format("hui PRIVMSG #pokerland :{0}", command));  
            } 
            */
        }
    }
}
