using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IRCBot
{
    /// <summary>
    /// Capable of accepting a user command and returning a string response.
    /// </summary>
    public abstract class Service
    {
        public MessageHandler handler;
        public Service(MessageHandler handler)
        {
            this.handler = handler;
        }
        
        public abstract Task AsyncHandleRequest(Match match, string response_target);
        public abstract Regex GetRegex();
        public abstract string GetDescription();        
    }
}
