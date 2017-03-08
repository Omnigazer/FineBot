using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace IRCBot
{
   public class MessageHandler
    {
        public IRCClient client;
        public List<Service> services;

        public MessageHandler(IRCClient client)
        {
            this.client = client;
            services = new List<Service>()
            {
             new DiceRoller(this),             
             new Help(this),
             new Memorizer(this),
             new Calculator(this)
            };
        }

        public async Task Respond(string message, string target)
        {
            client.SendMessage(message, target);
        }

        public void OutputMessage(string str)
        {
            Console.WriteLine(str);
        }
        
        public async Task HandleCommand(IRCClient.MessageArgs e)
        {
            await Task.Delay(500);
            foreach (Service service in services)
            {
                Match match = service.GetRegex().Match(e.Message);
                if (match.Success)
                {
                    string response_target = e.Target.StartsWith("#") ? e.Target : e.Sender;
                    try
                    {                        
                        await service.AsyncHandleRequest(match, response_target);
                    }
                    catch (Exception exception)
                    {
                        OutputMessage(exception.Message);
                        Respond("You broke me!", response_target);
                    }
                }
            }            
        }
    }
}
