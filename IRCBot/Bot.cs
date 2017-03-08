using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCBot
{
    public class Bot
    {
        public IRCClient client;
        public MessageHandler MessageHandler { get; set; }       

        public Bot(IRCClient client)
        {
            this.client = client;
            MessageHandler = new MessageHandler(client);
            client.NewPrivateMessage += HandleMessage;
            client.ConnectionRegistered += client_ConnectionRegistered;
        }

        void client_ConnectionRegistered(object sender, EventArgs e)
        {
            // stuff
        }

        /// <summary>
        /// Handle the event of a new message.
        /// </summary>        
        async void HandleMessage(object sender, IRCClient.MessageArgs e)
        {
            if (e.Sender != client.CurrentNick)
            {
                await MessageHandler.HandleCommand(e);
            }
        }            

        public void PrintInput(string str)
        {
            Console.Write(str);
        }

        /// <summary>
        /// Send a "private" message to a client or a channel.
        /// </summary>
        /// <param name="message">Message body.</param>
        /// <param name="recipient">Message recipient</param>
        public void SendMessage(string message, string recipient)
        {            
            client.SendMessage(message, recipient);
        }        
    }
}
