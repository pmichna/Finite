using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite
{
    public class Step
    {
        public string From { get; private set; }
        public string To { get; private set; }
        public char Over { get; private set; }
        public bool IsInit { get; set; }
        public bool IsFinal { get; set; }

        public Step(string from, string to, char over)
        {
            From = from;
            To = to;
            Over = over;
        }
    }
}
