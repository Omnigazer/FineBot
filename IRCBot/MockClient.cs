using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCBot
{
    class MockClient : IRCClient
    {
        public MockClient() : base(null, 0) { }

        public override void IssueCommand(string str)
        {
            Console.WriteLine("=> {0}", str);   
        }
    }
}
