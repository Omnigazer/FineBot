using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IRCBot
{
    class Calculator : Service
    {
        public Calculator(MessageHandler handler) : base(handler) { }

        /// <summary>
        /// Expression handlers grouped by priority
        /// </summary>
        public static class PriorityGroup
        {
            public static Regex GetRegex(int priority)
            {
                switch (priority)
                {
                    case 0:
                        {
                            return new Regex(@"(?<whole_thing>\((?<content>[^()]*)\))");                            
                        }
                    case 1:
                        {
                            return new Regex(@"(?<whole_thing>(?<number>\d+(,\d+){0,1})(?<operator>[\*/])(?<number2>\d+(,\d+){0,1}))");                            
                        }
                    case 2:
                        {
                            return new Regex(@"(?<whole_thing>(?<number>\d+(,\d+){0,1})(?<operator>[+\-])(?<number2>\d+(,\d+){0,1}))");                            
                        }                    
                    default:
                        return null;                        
                }                
            }
            public static string Calculate(int priority, Match match)
            {
                switch (priority)
                {
                    case 0:
                        {
                            return match.Groups["content"].Value;                            
                        }
                    case 1:
                        {
                            if (match.Groups["operator"].Value == "*")
                            {
                                return (float.Parse(match.Groups["number"].Value) * float.Parse(match.Groups["number2"].Value)).ToString();  
                            }
                            else
                            {
                                return (float.Parse(match.Groups["number"].Value) / float.Parse(match.Groups["number2"].Value)).ToString();
                            }                            
                        }
                    case 2:
                        {
                            if (match.Groups["operator"].Value == "+")
                            {
                                return (float.Parse(match.Groups["number"].Value) + float.Parse(match.Groups["number2"].Value)).ToString(); 
                            }
                            else
                            {
                                return (float.Parse(match.Groups["number"].Value) - float.Parse(match.Groups["number2"].Value)).ToString();  
                            }
                        }                    
                    default:
                        return null;
                }
            }
        }

        public string Evaluate(string expression, int priority)
        {            
            Regex regex = PriorityGroup.GetRegex(priority);
            if (regex != null)
            {
                Match match = regex.Match(expression);
                if (match.Success)
                {
                    string result = PriorityGroup.Calculate(priority, match);
                    expression = expression.Replace(match.Groups["whole_thing"].Value, Evaluate(result, priority + 1));
                }
                else
                {
                    return Evaluate(expression, priority + 1);
                }
                return Evaluate(expression, priority);
            }
            else
            {
                return expression;
            }
        }

        public override async Task AsyncHandleRequest(Match match, string target)
        {
            string result = Evaluate(match.Groups["expression"].Value.Replace(" ",""), 0);
            try
            {
                float.Parse(result);
                await handler.Respond((String.Format("I got {0}.", result)),target);
            }
            catch
            {
                handler.Respond(String.Format("{0}. No idea what that means, figure it out yourself.", result),target);
            }
        }

        public override Regex GetRegex()
        {
            return new Regex(@"!calc (?<expression>[\d,+\-\*/\(\) ]+)");
        }

        public override string GetDescription()
        {
            return "!calc <expression> - calculates a simple arithmetic expression.";
        }
    }
}
