using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Interfaces;
using GraphVizWrapper.Queries;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace Finite
{
    /// <summary>
    /// Interaction logic for OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : Window
    {
        private DFA _dfa;
        private DFABuilder _dfaBuilder;
        private string _beginDot = "digraph finite_state_machine { rankdir=LR; size=\"7,5\" ";
        private string _contentDot;
        private string _endDot = "}";
        private int _stepCounter = 0;
        private List<Step> _steps;
        private MainViewModel _mainViewModel;

        public class MainViewModel
        {
            private ObservableCollection<Transition> _transitions = new ObservableCollection<Transition>();
            private ObservableCollection<Tuple<string, string>> _labels = new ObservableCollection<Tuple<string, string>>();

            public MainViewModel()
            {
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

        public OutputWindow(DFABuilder dfaBuilder, bool stepByStepMode)
        {
            InitializeComponent();
            _dfa = dfaBuilder.Dfa;
            _mainViewModel = new MainViewModel();
            _dfaBuilder = dfaBuilder;
            this.DataContext = _mainViewModel;
            _steps = dfaBuilder.Steps;
            if (stepByStepMode)
            {
                btnNextStep.IsEnabled = true;
            }
            else
            {
                btnNextStep.IsEnabled = false;
                generateContentDot(_steps.Count);
                BitmapImage bmp = dot2bmp(_beginDot + _contentDot + _endDot);
                imgGraph.Source = bmp;
                foreach (Transition t in _dfa.Transitions)
                {
                    _mainViewModel.Transitions.Add(t);
                }
                foreach (State s in _dfa.States)
                {
                    _mainViewModel.Labels.Add(Tuple.Create(s.QLabel, s.RegexLabel));
                }
            }
        }

        private void generateContentDot(int numOfSteps)
        {
            // Building dot string
            StringBuilder content = new StringBuilder();

            // Generating string with final states (to mark them with double circle
            if (numOfSteps == _steps.Count)
            {
                StringBuilder sbFinalStates = new StringBuilder();
                foreach (State state in _dfa.FinalStates)
                {
                    sbFinalStates.Append("\"");
                    sbFinalStates.Append(state.QLabel);
                    sbFinalStates.Append("\" ");
                }
                sbFinalStates.Append("; ");
                content.Append("node [shape = doublecircle]; ");
                content.Append(sbFinalStates.ToString());
            };
            content.Append("node [shape = circle]; ");

            //Appending transistions
            for (int i = 0; i < numOfSteps; i++)
            {
                State from = _dfa.States.LastOrDefault(s => s.RegexLabel == _steps[i].From.RegexLabel);
                State to = _dfa.States.LastOrDefault(s => s.RegexLabel == _steps[i].To.RegexLabel);
                content.Append(from.QLabel);
                content.Append(" -> ");
                content.Append(to.QLabel);
                content.Append(" [ label = \"");
                content.Append(_steps[i].Over);
                content.Append("\" ]; ");
            }
            _contentDot = content.ToString();
        }

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
            if (_stepCounter == _steps.Count)
                return;
            if (_stepCounter == _steps.Count - 1)
                btnNextStep.IsEnabled = false;
            _mainViewModel.Transitions.Add(_dfa.Transitions[_stepCounter]);
            string from = _steps[_stepCounter].From.RegexLabel;
            string to = _steps[_stepCounter].To.RegexLabel;
            char over = _steps[_stepCounter].Over;
            bool isFromPresent, isToPresent;
            isFromPresent = isToPresent = false;
            foreach(Tuple<string, string> l in _mainViewModel.Labels)
            {
                if (l.Item2 == from)
                    isFromPresent = true;
                if (l.Item2 == to)
                    isToPresent = true;
            }
            if(!isFromPresent)
                _mainViewModel.Labels.Add(Tuple.Create(_steps[_stepCounter].From.QLabel, _steps[_stepCounter].From.RegexLabel));
            if(!isToPresent)
                _mainViewModel.Labels.Add(Tuple.Create(_steps[_stepCounter].To.QLabel, _steps[_stepCounter].To.RegexLabel));
            generateContentDot(++_stepCounter);
            BitmapImage bmp = dot2bmp(_beginDot + _contentDot + _endDot);
            imgGraph.Source = bmp;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var simulationWindow = new SimulationWindow(_dfaBuilder);
            simulationWindow.Show();
            
        }
    }
}
