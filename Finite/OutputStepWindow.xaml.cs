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

namespace Finite
{
    /// <summary>
    /// Interaction logic for OutputStepWindow.xaml
    /// </summary>
    public partial class OutputStepWindow : Window
    {
        private DFAStepBuilder _dfaStepBuilder = new DFAStepBuilder();
        private List<Step> _steps = new List<Step>();
        private string beginStr = "digraph finite_state_machine { rankdir=LR; size=\"8,5\" ";
        private string contentStr = "";
        private string endStr = "}";
        private int stepCounter = 0;
        private DFA _dfa;
        private class RegexLabel
        {
            public string Regular_expression { get; private set; }
            public string Label { get; private set; }
            public RegexLabel(string re, string label)
            {
                Regular_expression = re;
                Label = label;
            }
        }
        private class Trans
        {
            public string Initial_expression { get; private set; }
            public char Subtraction { get; private set; }
            public string Result { get; private set; }
            public Trans(string init, char sub, string res)
            {
                Initial_expression = init;
                Subtraction = sub;
                Result = res;
            }
        }
        private HashSet<RegexLabel> _gridLabels = new HashSet<RegexLabel>();
        private List<Trans> _gridTrans = new List<Trans>();

        public OutputStepWindow(string re)
        {
            InitializeComponent();
            _steps = _dfaStepBuilder.buildStepDFA(re);
            dataLabels.ItemsSource = _gridLabels;
            dataTrans.ItemsSource = _gridTrans;
            _dfa = DFABuilder.buildDFA(re);
        }

        private void btnStep_Click(object sender, RoutedEventArgs e)
        {
            // State numbering
            int counter = 1;
            State[] arrayOfStates = _dfa.States.ToArray();
            string[] arrayOfStrings = new string[arrayOfStates.Length];
            for (int i = 0; i < arrayOfStates.Length; i++)
            {
                if (arrayOfStates[i] == _dfa.InitState)
                {
                    arrayOfStrings[i] = "q0";
                }
                else
                {
                    arrayOfStrings[i] = "q" + counter++;
                }
            }

            if (stepCounter < _steps.Count)
            {
                string labelFrom = "";
                string labelTo = "";


                for (int i = 0; i < arrayOfStates.Length; i++)
                {
                    if (arrayOfStates[i].Label.Equals(_steps[stepCounter].From))
                    {
                        labelFrom = arrayOfStrings[i];
                    }
                    if (arrayOfStates[i].Label.Equals(_steps[stepCounter].To))
                        labelTo = arrayOfStrings[i];
                }


                contentStr += "\"" + labelFrom + "\"" + " -> " + "\"" + labelTo + "\"" + " [ label = \"" + _steps[stepCounter].Over + "\" ]; ";
                stepCounter++;
                beginStr += "node[shape = circle]; ";
                string dotString = beginStr + contentStr + endStr;

                var getStartProcessQuery = new GetStartProcessQuery();
                var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
                var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);
                var wrapper = new GraphVizWrapper.GraphVizWrapper(getStartProcessQuery, getProcessStartInfoQuery, registerLayoutPluginCommand);

                //Graph generating
                byte[] output = wrapper.GenerateGraph(dotString, GraphVizWrapper.Enums.GraphReturnType.Png);

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
                image.Freeze();
                imgGraph.Source = image;
            }
            else
            {
                // Generating string with final states (to mark them with double circle
                StringBuilder sbFinalStates = new StringBuilder();
                foreach (State state in _dfa.FinalStates)
                {
                    string final = "";
                    for (int i = 0; i < arrayOfStates.Length; i++)
                    {
                        if (state == arrayOfStates[i])
                            final = arrayOfStrings[i];
                    }
                    sbFinalStates.Append("\"");
                    sbFinalStates.Append(final);
                    sbFinalStates.Append("\" ");
                }
                sbFinalStates.Append("; ");
                beginStr += "node [shape = doublecircle]; ";
                beginStr += sbFinalStates.ToString();
                beginStr += " node[shape = circle];";

                string dotString = beginStr + contentStr + endStr;

                var getStartProcessQuery = new GetStartProcessQuery();
                var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
                var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);
                var wrapper = new GraphVizWrapper.GraphVizWrapper(getStartProcessQuery, getProcessStartInfoQuery, registerLayoutPluginCommand);

                //Graph generating
                byte[] output = wrapper.GenerateGraph(dotString, GraphVizWrapper.Enums.GraphReturnType.Png);

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
                image.Freeze();
                imgGraph.Source = image;
            }
        }
    }
}
