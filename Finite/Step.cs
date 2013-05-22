using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite
{
    public class Step
    {
        //public string FromRegex { get; private set; }
        //public string ToRegex { get; private set; }
        //public char Over { get; private set; }
        //public string FromQLabel { get; set; }
        //public string ToQLabel { get; set; }

        public State From { get; private set; }
        public State To { get; private set; }
        public char Over { get; private set; }

        public Step(State from, State to, char over)
        {
            From = from;
            To = to;
            Over = over;
        }
    }
}
