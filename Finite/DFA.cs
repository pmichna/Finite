using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite
{
    public class DFA
    {
        public HashSet<State> States { get; private set; }
        public State InitState { get; set; }

        public DFA(State s)
        {
            States = new HashSet<State>();
            InitState = s;
            States.Add(InitState);
        }
    }
}
