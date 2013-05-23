using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite
{
    public class State
    {
        public Dictionary<char, State> transistions = new Dictionary<char, State>();
        public string RegexLabel { get; set; }
        public string QLabel { get; set; }
        public bool IsFinal { get; private set; }

        public State(RegularExpression re, bool isFinal)
        {
            RegexLabel = re.Value;
            IsFinal = isFinal;
        }

        public bool addTransition(State to, char trans)
        {
            bool present = false;
            foreach(KeyValuePair<char,State> s in transistions)
            {
                if (s.Value.RegexLabel == to.RegexLabel && s.Key == trans)
                {
                    present = true;
                    break;
                }
            }
            if (!present)
            {
                transistions.Add(trans, to);
                return true;
            }
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            State state = obj as State;
            if ((Object)state == null)
                return false;
            return RegexLabel.Equals(state.RegexLabel);
        }

        public override int GetHashCode()
        {
            int result=1;
            foreach (char c in RegexLabel)
                result *= (c+1);
            return result;
        }

        public override string ToString()
        {
            return RegexLabel;
        }
    }
}
