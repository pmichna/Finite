using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite
{
    public class DFABuilder
    {
        public List<Step> Steps { get; private set; }
        public DFA Dfa { get; private set; }

        public DFABuilder()
        {
            Steps = new List<Step>();
        }

        private static RegularExpression CreateEmptySet()
        {
            return new RegularExpression(RegularExpression.EMPTY_SET);
        }

        //helper function
        private static RegularExpression v(RegularExpression re)
        {
            RegularExpression r, s;

            if (re.IsEmptyWord)
            {
                return new RegularExpression();
            }
            else if ((!re.IsEmptyWord && re.Value.Length == 1) || re.IsEmptySet)
            {
                return CreateEmptySet();
            }
            else if (re.IsConcatenation)
            {
                re.GetConcatSubExpressions(out r, out s);
                if (v(r).IsEmptyWord && v(s).IsEmptyWord)
                    return new RegularExpression();
            }
            else if (re.IsUnion)
            {
                re.GetUnionSubExpressions(out r, out s);
                if (v(r).IsEmptyWord || v(s).IsEmptyWord)
                    return new RegularExpression();
            }
            else if (re.IsKleene)
            {
                return new RegularExpression();
            }
            return CreateEmptySet();
        }

        public static RegularExpression Derive(RegularExpression re, char a)
        {
            RegularExpression r, s;
            if (re.IsEmptyWord)
            {
                return CreateEmptySet();
            }
            else if (re.Value == a.ToString())
            {
                return new RegularExpression();
            }
            else if (re.Value.Length == 1 && !(re.Value == a.ToString()))
            {
                return CreateEmptySet();
            }
            else if (re.Value.Equals(RegularExpression.EMPTY_SET))
            {
                return CreateEmptySet();
            }
            else if (re.IsConcatenation)
            {
                re.GetConcatSubExpressions(out r, out s);
                return Derive(r, a).Concatenate(s).Union(v(r).Concatenate(Derive(s, a)));
            }
            else if (re.IsKleene)
            {
                string expUnderStar;
                if (re.Value[0] == '(')
                {
                    expUnderStar = re.Value.Substring(1, re.Value.Length - 3);
                }
                else
                {
                    expUnderStar = re.Value[0].ToString();
                }
                return Derive(new RegularExpression(expUnderStar), a).Concatenate(new RegularExpression(re.Value));
            }
            else if (re.IsPlus)
            {
                string expUnderPlus;
                if (re.Value[0] == '(')
                {
                    expUnderPlus = re.Value.Substring(1, re.Value.Length - 4);
                }
                else
                {
                    expUnderPlus = re.Value[0].ToString();
                }
                return Derive(new RegularExpression(expUnderPlus), a).Concatenate(new RegularExpression("(" + expUnderPlus + ")" + "*"));
            }
            else if (re.IsUnion)
            {
                re.GetUnionSubExpressions(out r, out s);
                return Derive(r, a).Union(Derive(s, a));
            }

            return null; //indicates error
        }

        public void buildDFA(string re)
        {
            Dfa = new DFA(new RegularExpression(re));
            HashSet<char> alphabet = new HashSet<char>();
            foreach (char c in re)
            {
                if (Char.IsLetter(c))
                    alphabet.Add(c);
            }
            var newStates = new HashSet<State>();

            int added = 0;
            //int qcounter = 1;
            do
            {
                added = 0;
                newStates.Clear();
                foreach (State state in Dfa.States)
                {
                    foreach (char c in alphabet)
                    {
                        RegularExpression newRegEx = Derive(new RegularExpression(state.RegexLabel), c);
                        State newState = new State(newRegEx, IsExpressionFinal(newRegEx));
                        newStates.Add(newState);
                        Dfa.addTransition(state.RegexLabel, newState.RegexLabel, c);
                        Step newStep = new Step(state, newState, c);

                        //bool stateExistsInDfa = false;
                        //foreach (State s in Dfa.States)
                        //{
                        //    if (s.RegexLabel == newState.RegexLabel)
                        //    {
                        //        stateExistsInDfa = true;
                        //        break;
                        //    }
                        //}

                        //if (!stateExistsInDfa)
                        //{
                        //    newState.QLabel = "q" + qcounter++.ToString();
                        //}
                        if (!stepExists(newStep))
                        {
                            Steps.Add(newStep);
                        }
                    }
                }

                foreach (State state in newStates)
                {
                    if (Dfa.addState(state))
                    {
                        added++;
                    }
                }
            } while (added != 0);
        }

        //private bool stateExists(State state)
        //{
        //    foreach (State s in Dfa.States)
        //    {
        //        if (s.RegexLabel == state.RegexLabel)
        //            return true;
        //    }
        //    return false;
        //}

        private bool stepExists(Step step)
        {
            foreach (Step s in Steps)
            {
                if (s.From.RegexLabel == step.From.RegexLabel &&
                    s.To.RegexLabel == step.To.RegexLabel &&
                    s.Over == step.Over)
                    return true;
            }
            return false;
        }

        public static bool IsExpressionFinal(RegularExpression re)
        {
            string str1 = v(re).Value;
            string str2 = RegularExpression.EMPTY_WORD.ToString();
            if (str1 == str2)
            {
                return true;
            }
            return false;
        }
    }
}
