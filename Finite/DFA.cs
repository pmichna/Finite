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

        public DFA(RegularExpression re)
        {
            States = new HashSet<State>();
            InitState = new State(re.Value);
            States.Add(InitState);
        }

        public void addState(State state)
        {
            States.Add(state);
        }

        public void addTransition(RegularExpression from, RegularExpression to, char transition)
        {
            State stateFrom = States.First(state => state.Label == from.Value);
            State stateTo = States.First(state => state.Label == to.Value);
            stateFrom.addTransition(stateTo, transition);
        }

    }
}
