using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite
{
    public class State
    {
        private Dictionary<char, State> transistions = new Dictionary<char, State>();
        public bool IsFinal { get; set; }
        public string Label { get; set; }

        public State(string s)
        {
            Label = s;
        }

        public void addTransition(State to, char trans)
        {
            transistions.Add(trans, to);
        }
    }
}
