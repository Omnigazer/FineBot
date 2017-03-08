using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IRCBot
{
    public class IRCClient : ITextInterface
    {
        public string user = "test host server :Epifan Pahomov";
        public string nick = "finebot";
        public string pass = "pass";
        public string current_prefix = null;


        Client client;
        public Encoding current_encoding;

        // public Dictionary<string, TaskCompletionSource<WhoisInfo>> pending_whois;

        public event EventHandler ConnectionRegistered = delegate { };
        public event EventHandler<JoinArgs> JoinedChannel = delegate { };
        public event EventHandler<JoinArgs> LeftChannel = delegate { };
        public event EventHandler<StringArgs> NewMessage = delegate { };
        public event EventHandler<MessageArgs> NewPrivateMessage = delegate { };
        public event EventHandler<MessageArgs> NewAction = delegate { };
        public event EventHandler<StringArgs> SendingCommand = delegate { };

        public string CurrentNick
        {
            get
            {
                return "finebot";
            }
        }

        public IRCClient(IPAddress ipAddress, int port)
        {
            current_encoding = Encoding.GetEncoding(1251);
            // pending_whois = new Dictionary<string, TaskCompletionSource<WhoisInfo>>();
            client = new Client(ipAddress, port);
            client.client_interface = this;            
        }

        public void HandleCommand(string command)
        {
            if (command.StartsWith("PING"))
            {
                string response = "PONG ";
                response += command.Split(' ')[1];
                IssueCommand(response);                
            }
            else
            {
                var args = command.Split(' ');
                string command_header = args[1];
                switch (command_header)
                {
                    case "001":
                        {                            
                            ConnectionRegistered(this, EventArgs.Empty);
                            break;
                        }
                    case "311":
                        {                            
                            string whois_nick = args[3];
                            string ident = args[4];
                            string host = args[5];
                            string name = args[7].Substring(1);
                            // stub
                            /*
                            WhoisInfo info = new WhoisInfo(whois_nick,name,ident);
                            if (pending_whois[whois_nick] != null)
                            {
                                pending_whois[whois_nick].SetResult(info);                                
                            }                            
                            */
                            break;
                        }
                    case "401":
                        {
                            string whois_nick = args[3];
                            /*
                            if (pending_whois.ContainsKey(whois_nick))
                            {
                                pending_whois[whois_nick].SetResult(null);
                            }
                            */
                            break;
                        }
                    case "JOIN":
                        {
                            Regex regex = new Regex(@"^(?<msg_sender>:(?<nick>.+?)!.+?) JOIN :?(?<channel>.+?)$");
                            Match match = regex.Match(command);
                            string nick = match.Groups["nick"].Value;
                            string channel = match.Groups["channel"].Value;
                            JoinedChannel(this, new JoinArgs(nick, channel));
                            break;
                        }
                    case "PART":
                        {
                            Regex regex = new Regex(@"^(?<msg_sender>:(?<nick>.+?)!.+?) PART (?<channel>.+?)$");
                            Match match = regex.Match(command);
                            string nick = match.Groups["nick"].Value;
                            string channel = match.Groups["channel"].Value;
                            LeftChannel(this, new JoinArgs(nick, channel));
                            break;
                        }
                    case "PRIVMSG":
                        {
                            Regex regex = new Regex(@"^(?<msg_sender>:(?<msg_nick>.+?)!.+?) PRIVMSG (?<msg_target>.+?) :(?<msg_body>.*)");
                            Match match = regex.Match(command);                            
                            string msg_sender = match.Groups["msg_nick"].Value;
                            string msg_target = match.Groups["msg_target"].Value;
                            string msg_body = match.Groups["msg_body"].Value;
                            NewPrivateMessage(this, new MessageArgs(msg_sender, msg_target, msg_body));                           
                            break;
                        }
                    default:
                        {
                            NewMessage(this, new StringArgs(command));
                            break;
                        }
                }
            }
        }

        /*
        public async Task<WhoisInfo> Whois(string nick)
        {            
            if (!pending_whois.ContainsKey(nick))
            {
                IssueCommand(String.Format("WHOIS {0}", nick));
                TaskCompletionSource<WhoisInfo> tcs = new TaskCompletionSource<WhoisInfo>();
                pending_whois.Add(nick, tcs);
                WhoisInfo response = await tcs.Task;
                pending_whois.Remove(nick);
                return response;
            }
            else
            {
                return await pending_whois[nick].Task;
            }            
        } 
        */       
        
        public class MessageArgs : EventArgs
        {            
            public string Sender { get; set; }
            public string Target { get; set; }
            public string Message { get; set; }
            public MessageArgs(string sender, string target, string message)
            {                
                Sender = sender;
                Target = target;
                Message = message;
            }
        }

        public class StringArgs : EventArgs
        {            
            public string Message { get; set; }
            public StringArgs(string message)
            {                
                Message = message;
            }
        }

        public class JoinArgs : EventArgs
        {
            public string Nick { get; set; }
            public string Channel { get; set; }
            public JoinArgs(string nick, string channel)
            {                
                Nick = nick;
                Channel = channel;
            }
        }

        // should be protected, accepting verbose commands for now
        public virtual void IssueCommand(string str)
        {            
            SendingCommand(this, new StringArgs(str));
            client.Send(current_encoding.GetBytes(str + "\r\n"));
        }                

        public void SendMessage(string message, string recipient)
        {
            // str = String.Format("PRIVMSG {0} :{1}", current_prefix, str);
            string command_text = String.Format("PRIVMSG {0} :{1}", recipient, message);
            IssueCommand(command_text);
            NewPrivateMessage(this, new MessageArgs(CurrentNick, recipient, message));
        }

        public void SendAction(string message, string recipient)
        {
            string command_text = String.Format("PRIVMSG {0} :\x0001ACTION {1}\x0001", recipient, message);
            IssueCommand(command_text);
            NewAction(this, new MessageArgs(CurrentNick, recipient, message));
        }

        public void Join(string channel)
        {
            string command_text = String.Format("JOIN {0}", channel);
            IssueCommand(command_text);
        }

        public void Part(string channel)
        {
            string command_text = String.Format("PART {0}", channel);
            IssueCommand(command_text);            
        }

        public void RegisterConnection()
        {
            IssueCommand(String.Format("USER {0}", user));
            IssueCommand(String.Format("NICK {0}", nick));
            IssueCommand(String.Format("PASS {0}", pass));
        }

        public void Quit()
        {
            IssueCommand("QUIT");
        }

        public async Task Start()
        {
            await client.Start();
            RegisterConnection();
        }            
    }
}
