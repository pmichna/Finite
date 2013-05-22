using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Interfaces;
using GraphVizWrapper.Queries;
using System.IO;
using System.Collections.ObjectModel;

namespace Finite
{
    public partial class SimulationWindow : Window
    {
        private DFABuilder _dfaBuilder;
        private DFA _dfa;
        private List<Step> _steps;
        private State _currentState;
        private int _currentChar;
        private string _word;

        public SimulationWindow(DFABuilder dfaBuilder)
        {
            InitializeComponent();
            _dfaBuilder = dfaBuilder;
            _dfa = _dfaBuilder.Dfa;
            _steps = _dfaBuilder.Steps;
            this.DataContext = new MainViewModel(_dfa);
        }

        public class MainViewModel
        {
            private ObservableCollection<Transition> _transitions = new ObservableCollection<Transition>();
            private ObservableCollection<Tuple<string, string>> _labels = new ObservableCollection<Tuple<string, string>>();

            public MainViewModel(DFA dfa)
            {
                foreach (Transition t in dfa.Transitions)
                {
                    _transitions.Add(t);
                }
                foreach(State s in dfa.States)
                {
                    _labels.Add(Tuple.Create(s.QLabel, s.RegexLabel));
                }
            }
            public ObservableCollection<Transition> Transitions
            {
                get { return _transitions; }
                set
                {
                    _transitions = value;
                }
            }
            public ObservableCollection<Tuple<string, string>> Labels
            {
                get { return _labels; }
                set
                {
                    _labels = value;
                }
            }
        }

        private void btnStartSimulation_Click(object sender, RoutedEventArgs e)
        {
            _word = txtWord.Text;
            foreach (char c in _word)
            {
                if (!_dfa.Alphabet.Contains(c))
                {
                    MessageBox.Show("Your word contains illegal characters or letters, that do not belong to the alphabet.");
                    return;
                }
            }
            _currentChar = 0;
            _currentState = _dfa.InitState;
            string dot = generateDot();
            BitmapImage bmp = dot2bmp(dot);
            imgGraph.Source = bmp;
            btnNextStep.IsEnabled = true;
            btnStartSimulation.IsEnabled = false;
            if (_word.Length == 1)
            {
                if (_currentState.IsFinal)
                {
                    MessageBox.Show("The word \"" + _word + "\" is accepted.");
                }
                else
                {
                    MessageBox.Show("The word " + _word + " is not accepted.");
                }
                btnNextStep.IsEnabled = false;
                btnStartSimulation.IsEnabled = true;
            }
        }

        private string generateDot()
        {
            State newCurrentState = null;
            // find new current state
            foreach (Transition t in _dfa.Transitions)
            {
                if (t.From == _currentState.RegexLabel && t.Over == _word[_currentChar])
                {
                    foreach (State s in _dfa.States)
                    {
                        if (s.RegexLabel == t.To)
                        {
                            newCurrentState = s;
                            break;
                        }
                    }
                    break;
                }
            }

            StringBuilder sbFsm = new StringBuilder();
            sbFsm.Append("digraph finite_state_machine { rankdir=LR; size=\"7,5\" ");
            StringBuilder sbFinalStates = new StringBuilder();
            foreach (State state in _dfa.FinalStates)
            {
                sbFinalStates.Append("\"");
                sbFinalStates.Append(state.QLabel);
                sbFinalStates.Append("\" ");
            }
            //sbFinalStates.Append("; ");
            sbFsm.Append("node [shape = doublecircle]; ");
            sbFsm.Append(sbFinalStates.ToString());
            sbFsm.Append("node [shape = circle]; ");
            for (int i = 0; i < _dfaBuilder.Steps.Count; i++)
            {
                State from = _dfa.States.LastOrDefault(s => s.RegexLabel == _steps[i].From.RegexLabel);
                State to = _dfa.States.LastOrDefault(s => s.RegexLabel == _steps[i].To.RegexLabel);
                sbFsm.Append(from.QLabel);
                sbFsm.Append(" -> ");
                sbFsm.Append(to.QLabel);
                sbFsm.Append(" [ label = \"");
                sbFsm.Append(_steps[i].Over);
                sbFsm.Append("\"");
                if(from.RegexLabel == _currentState.RegexLabel && to.RegexLabel == newCurrentState.RegexLabel && _steps[i].Over == _word[_currentChar])
                    sbFsm.Append(", color=red");
                sbFsm.Append("] ; ");

            }
            sbFsm.Append("\"" + _currentState.QLabel + "\" [color=red];");
            sbFsm.Append("\"" + newCurrentState.QLabel + "\" [color=red]");
            sbFsm.Append("}");
            _currentChar++;
            _currentState = newCurrentState;
            return sbFsm.ToString();
        }

        //private string generateNextStepDot()
        //{
        //    StringBuilder sbFsm = new StringBuilder();
        //    sbFsm.Append("digraph finite_state_machine { rankdir=LR; size=\"7,5\" ");
        //    StringBuilder sbFinalStates = new StringBuilder();
        //    foreach (State state in _dfa.FinalStates)
        //    {
        //        sbFinalStates.Append("\"");
        //        sbFinalStates.Append(state.QLabel);
        //        sbFinalStates.Append("\" ");
        //    }
        //    sbFinalStates.Append("; ");
        //    sbFsm.Append("node [shape = doublecircle]; ");
        //    sbFsm.Append(sbFinalStates.ToString());
        //    sbFsm.Append("node [shape = circle]; ");
        //    for (int i = 0; i < _dfaBuilder.Steps.Count; i++)
        //    {
        //        State from = _dfa.States.LastOrDefault(s => s.RegexLabel == _steps[i].From.RegexLabel);
        //        State to = _dfa.States.LastOrDefault(s => s.RegexLabel == _steps[i].To.RegexLabel);
        //        sbFsm.Append(from.QLabel);
        //        sbFsm.Append(" -> ");
        //        sbFsm.Append(to.QLabel);
        //        sbFsm.Append(" [ label = \"");
        //        sbFsm.Append(_steps[i].Over);
        //    }
        //    sbFsm.Append("}");
        //    return sbFsm.ToString();
        //}

        private BitmapImage dot2bmp(string dot)
        {
            // Starting wrapper
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);
            var wrapper = new GraphVizWrapper.GraphVizWrapper(getStartProcessQuery, getProcessStartInfoQuery, registerLayoutPluginCommand);

            //Graph generating
            byte[] output = wrapper.GenerateGraph(dot, GraphVizWrapper.Enums.GraphReturnType.Png);

            //Conversion to BitmapImage
            var image = new BitmapImage();
            using (var mem = new MemoryStream(output))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }

            return image;
        }

        private void btnNextStep_Click(object sender, RoutedEventArgs e)
        {
            if ( _currentChar == _word.Length - 1)
            {
                BitmapImage nextBmp = dot2bmp(generateDot());
                imgGraph.Source = nextBmp;
                if (_currentState.IsFinal)
                {
                    MessageBox.Show("The word " + _word + " is accepted.");
                }
                else
                {
                    MessageBox.Show("The word " + _word + " is not accepted.");
                }
                btnNextStep.IsEnabled = false;
                btnStartSimulation.IsEnabled = true;
                return;
            }
            else
            {
                BitmapImage nextBmp = dot2bmp(generateDot());
                imgGraph.Source = nextBmp;
            }
        }
    }
}
