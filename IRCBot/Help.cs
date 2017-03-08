using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IRCBot
{
    class Help : Service
    {
        public Help(MessageHandler handler) : base(handler) { }               

        public override Regex GetRegex()
        {
            return new Regex(@"^!help");
        }

        public override async Task AsyncHandleRequest(Match match, string target)
        {            
            for (int i = 0; i < handler.services.Count; i++)
            {
                string desc = handler.services[i].GetDescription();
                await handler.Respond(desc, target);                
            }
            
        }

        public override string GetDescription()
        {
            return "!help - displays this text";
        }
    }
}
