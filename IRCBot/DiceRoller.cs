using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IRCBot
{
    class DiceRoller : Service
    {
        Random rnd = new Random();

        public DiceRoller(MessageHandler handler) : base(handler) { }

        public override Regex GetRegex()
        {
            return new Regex(@"^!(?<dnumber>\d+)d(?<dsides>\d+)");
        }       

        public override async Task AsyncHandleRequest(Match match, string target)
        {
            var nod_string = match.Groups["dnumber"].Value;
            var spd_string = match.Groups["dsides"].Value;
            if (nod_string.Length > 2 || spd_string.Length > 2)
            {
                await handler.Respond("Nice try.", target);
                return;
            }
            int number_of_dice = Convert.ToInt32(nod_string);
            int sides_per_die = Convert.ToInt32(spd_string);
            int result = Roll(number_of_dice, sides_per_die);
            if (result == -1)
            {
                await handler.Respond("Nice try.", target);                
            }
            else
            {
                await handler.Respond(String.Format("{0}d{1} roll result : {2}", number_of_dice, sides_per_die, result), target);                
            }
        }

        public int Roll(int dice, int sides)
        {
            if (dice < 1 || dice >= 100 || sides <= 1 || sides >= 100)
            {
                return -1;
            }
            int sum = 0;            
            for (int i = 0; i < dice; i++)
            {
                sum += rnd.Next(sides) + 1;
            }
            return sum;
        }

        public override string GetDescription()
        {
            return "!<dice>d<sides> - rolls x dice with y sides";
        }
    }
}
