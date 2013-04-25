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
        public bool IsFinal { get; set; }
        public string Label { get; set; }

        public State(string s)
        {
            Label = s;
        }

        public void addTransition(State to, char trans)
        {
            bool present = false;
            foreach(KeyValuePair<char,State> s in transistions)
            {
                if (s.Value.Label == to.Label && s.Key == trans)
                    present = true;
            }
            if(!present)
                transistions.Add(trans, to);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            State state = obj as State;
            if ((Object)state == null)
                return false;
            return Label.Equals(state.Label);
        }

        public override int GetHashCode()
        {
            int result=1;
            foreach (char c in Label)
                result *= c;
            return result;
        }
    }
}
