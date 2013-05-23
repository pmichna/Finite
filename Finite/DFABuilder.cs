using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            var newStates = new HashSet<State>();
            var newTransitions = new List<Transition>();
            var newSteps = new List<Step>();

            int added = 0;
            do
            {
                added = 0;
                newStates.Clear();
                newTransitions.Clear();
                newSteps.Clear();
                foreach (State state in Dfa.States)
                {
                    foreach (char c in Dfa.Alphabet)
                    {
                        RegularExpression newRegEx = Derive(new RegularExpression(state.RegexLabel), c);
                        State newState = null;
                        newState = new State(newRegEx, IsExpressionFinal(newRegEx));
                        newStates.Add(newState);
                        newTransitions.Add(new Transition(state.RegexLabel, newState.RegexLabel, c));
                        Step newStep = new Step(state, newState, c);
                        newSteps.Add(newStep);
                    }
                }

                foreach (State state in newStates)
                {
                    bool hasSubstring = false;
                    foreach (State s1 in Dfa.States)
                    {
                        if(state.RegexLabel.Contains(s1.RegexLabel) && state.RegexLabel != s1.RegexLabel)
                        {
                            hasSubstring = true;
                            break;
                        }
                    }
                    State equivalent = null;
                    if (hasSubstring)
                    {
                        var testWindow = new UserInteraction(state.RegexLabel, Dfa.States);
                        if (testWindow.ShowDialog() == false)
                        {
                            equivalent = testWindow.GetSelectedState();
                        }
                        if (equivalent == null)
                        {
                            if (Dfa.addState(state))
                            {
                                added++;
                            }
                        }
                        else
                        {
                            // update transitions
                            foreach (Transition t in Dfa.Transitions)
                            {
                                if (t.From == state.RegexLabel)
                                    t.From = equivalent.RegexLabel;
                                if (t.To == state.RegexLabel)
                                    t.To = equivalent.RegexLabel;
                            }
                            foreach (Transition t in newTransitions)
                            {
                                if (t.From == state.RegexLabel)
                                    t.From = equivalent.RegexLabel;
                                if (t.To == state.RegexLabel)
                                    t.To = equivalent.RegexLabel;
                            }
                            //update steps
                            foreach (Step step in Steps)
                            {
                                if (step.From.RegexLabel == state.RegexLabel)
                                    step.From.RegexLabel = equivalent.RegexLabel;
                                if (step.To.RegexLabel == state.RegexLabel)
                                    step.To.RegexLabel = equivalent.RegexLabel;
                            }
                            foreach (Step step in newSteps)
                            {
                                if (step.From.RegexLabel == state.RegexLabel)
                                    step.From.RegexLabel = equivalent.RegexLabel;
                                if (step.To.RegexLabel == state.RegexLabel)
                                    step.To.RegexLabel = equivalent.RegexLabel;
                            }
                            if (Dfa.addState(equivalent))
                            {
                                
                                added++;
                            }
                        }
                    }
                    if (Dfa.addState(state))
                    {

                        added++;
                    }
                }
                foreach (Transition t in newTransitions)
                {
                    bool isTransitionAlreadyPresent = false;

                    foreach (Transition t1 in Dfa.Transitions)
                    {
                        if (t1.From == t.From && t1.To == t.To && t.Over == t1.Over)
                        {
                            isTransitionAlreadyPresent = true;
                            break;
                        }
                    }

                    if (isTransitionAlreadyPresent)
                        continue;
                    Dfa.Transitions.Add(t);
                }

                foreach (Step step in newSteps)
                {

                    bool isStepAlreadyPresent = false;

                    foreach (Step s1 in Steps)
                    {
                        if (step.From.RegexLabel == s1.From.RegexLabel && step.To.RegexLabel == s1.To.RegexLabel && step.Over == s1.Over)
                        {
                            isStepAlreadyPresent = true;
                            break;
                        }
                    }

                    if (isStepAlreadyPresent)
                        continue;
                    Steps.Add(step);
                }

            } while (added != 0);
        }

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
