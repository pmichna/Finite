using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite
{
    public class DFA
    {
        public List<State> States { get; private set; }
        public List<Transition> Transitions { get; private set; } // tuple = <from, to, transition_character>
        public State InitState { get; set; }
        public HashSet<char> Alphabet { get; private set; }
        private int qcounter = 1;

        public DFA(RegularExpression re)
        {
            States = new List<State>();
            InitState = new State(re, DFABuilder.IsExpressionFinal(re));
            InitState.QLabel = "q0";
            Transitions = new List<Transition>();
            States.Add(InitState);
            Alphabet = new HashSet<char>();
            foreach (char c in re.Value)
            {
                if (Char.IsLetter(c))
                    Alphabet.Add(c);
            }
        }

        public bool addState(State state)
        {
            if (isStatePresent(state))
            {
                return false;
            }
            state.QLabel = "q" + qcounter++.ToString();
            States.Add(state);
            return true;
        }

        private bool isStatePresent(State state)
        {
            foreach (State s in States)
            {
                if (s.RegexLabel == state.RegexLabel)
                    return true;
            }
            return false;
        }

        public bool addTransition(string from, string to, char transition)
        {
            foreach (Transition t in Transitions)
            {
                if (t.From == from && t.To == to && t.Over == transition)
                    return false;
            }
            Transitions.Add(new Transition(from, to, transition));
            return true;
        }

        //public void addTransition(RegularExpression from, RegularExpression to, char transition)
        //{
        //    State stateFrom = States.First(state => state.Label == from.Value);
        //    State stateTo = States.First(state => state.Label == to.Value);
        //    stateFrom.addTransition(stateTo, transition);
        //}

        public List<State> FinalStates
        {
            get
            {
                List<State> result = new List<State>();
                foreach (State state in States)
                {
                    if (state.IsFinal)
                        result.Add(state);
                }
                return result;
            }
        }

    }
}
