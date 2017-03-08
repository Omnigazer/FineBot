using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace IRCBot
{
    public class Memorizer : Service
    {
        string wtf = "What's there to memorize?", no_memo = "There's no such memo";
        string memo_path = "memos.txt";
        Random rnd;
        List<string> memos;

        public Memorizer(MessageHandler handler) : base(handler)
        {
            memos = new List<string>();
            rnd = new Random();
            if (File.Exists(memo_path))
            {
                var memo_strings = File.ReadAllLines(memo_path);
                foreach (var memo in memo_strings)
                {
                    memos.Add(memo);
                }
            }
        }

        public override async Task AsyncHandleRequest(Match match, string target)
        {
            if (match.Groups["memorize"].Length != 0)
            {
                if (match.Groups["text_to_memorize"].Length != 0)
                {
                    Memorize(match.Groups["text_to_memorize"].Value);
                    await handler.Respond("Got it, boss", target);
                }
                else
                {
                    await handler.Respond(wtf, target);
                }
            }
            else if (match.Groups["output_memo"].Length != 0)
            {
                if (match.Groups["memo_number"].Length > 0 && match.Groups["memo_number"].Length <= 7)
                {
                    await handler.Respond(GetMemo(Convert.ToInt32(match.Groups["memo_number"].Value)),target);
                }
                else
                {
                    await handler.Respond(no_memo, target);
                }
            }            
        }

        public void Memorize(string str)
        {
            memos.Add(str);
            if (!File.Exists(memo_path))
            {
                var file = File.Create(memo_path);
                file.Close();
            }
            using (var memo_stream = File.AppendText(memo_path))
            {
                memo_stream.WriteLine(str);
            }
            
        }

        public string GetMemo(int number)
        {
            if (number == 0)
            {
                return memos[(rnd.Next(memos.Count))];
            }
            else if (number <= memos.Count)
            {
                return memos[number - 1];
            }
            else
            {
                return no_memo;
            }
        }

        public override Regex GetRegex()
        {
            return new Regex(@"(?<memorize>^!memorize (?<text_to_memorize>.+))|(?<output_memo>!memo #(?<memo_number>\d+))");
        }

        public override string GetDescription()
        {
            return "!memorize <text> - memorizes the specified text. !memo #<number> - displays the specified memo.";
        }
    }
}
