using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite
{
    public class Transition
    {
        public string From { get; set; }
        public string To { get; set; }
        public char Over { get; set; }

        public Transition(string from, string to, char over)
        {
            From = from;
            To = to;
            Over = over;
        }
    }
}
