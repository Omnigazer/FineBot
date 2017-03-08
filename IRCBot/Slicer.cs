using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCBot
{
    class Slicer
    {
        public static string Slice(StringBuilder sb, StateObject state)
        {
            string str = sb.ToString();
            string command = "";            
            
                if (str.IndexOf("\r\n") >= 0)
                {
                    int crlf_index = str.IndexOf("\r\n");
                    command = str.Substring(0, crlf_index);
                    sb = sb.Remove(0, crlf_index + 2);
                }            
            return command;
        }
    }
}
