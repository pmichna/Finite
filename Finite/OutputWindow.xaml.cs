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

namespace Finite
{
    /// <summary>
    /// Interaction logic for OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : Window
    {
        private DFA _dfa;
        private class RegexLabel
        {
            public string Regular_expression {get; private set;}
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
        private List<RegexLabel> _gridLabels = new List<RegexLabel>();
        private List<Trans> _gridTrans = new List<Trans>();

        public OutputWindow(DFA dfa)
        {
            InitializeComponent();
            _dfa = dfa;
            imgGraph.Source = dfa2bmp(dfa);
            dataLabels.ItemsSource = _gridLabels;
            dataTrans.ItemsSource = _gridTrans;
        }

        private BitmapImage dfa2bmp(DFA dfa)
        {
            // State numbering
            int counter = 1;
            State[] arrayOfStates = dfa.States.ToArray();
            string[] arrayOfStrings = new string[arrayOfStates.Length];
            for (int i = 0; i < arrayOfStates.Length; i++)
            {
                if (arrayOfStates[i] == dfa.InitState)
                {
                    _gridLabels.Add(new RegexLabel(arrayOfStates[i].Label, "q0"));
                    arrayOfStrings[i] = "q0";
                }
                else
                {
                    _gridLabels.Add(new RegexLabel(arrayOfStates[i].Label, "q" + counter));
                    arrayOfStrings[i] = "q" + counter++;
                }
            }

            // Starting wrapper
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);
            var wrapper = new GraphVizWrapper.GraphVizWrapper(getStartProcessQuery, getProcessStartInfoQuery, registerLayoutPluginCommand);

            // Buildung dot string
            StringBuilder sbFsm = new StringBuilder();
            sbFsm.Append("digraph finite_state_machine { ");
            sbFsm.Append("rankdir=LR; ");
            sbFsm.Append("size=\"7,5\" ");
            // Generating string with final states (to mark them with double circle
            StringBuilder sbFinalStates = new StringBuilder();
            foreach (State state in dfa.FinalStates)
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
            sbFsm.Append("node [shape = doublecircle]; ");
            sbFsm.Append(sbFinalStates.ToString());
            sbFsm.Append("node [shape = circle]; ");
            //Appending all the transistions
            foreach (State state in dfa.States)
            {
                foreach (KeyValuePair<char, State> kv in state.transistions)
                {
                    State stateFrom = state;
                    State stateTo = kv.Value;
                    string labelFrom = "";
                    string labelTo = "";
                    for (int i = 0; i < arrayOfStates.Length; i++)
                    {
                        if (arrayOfStates[i].Label.Equals(stateFrom.Label))
                            labelFrom = arrayOfStrings[i];
                        if (arrayOfStates[i].Label.Equals(stateTo.Label))
                            labelTo = arrayOfStrings[i];
                    }
                    sbFsm.Append("\"" + labelFrom + "\"" + " -> " + "\"" + labelTo + "\"" + " [ label = \"" + kv.Key + "\" ]; ");
                    _gridTrans.Add(new Trans(stateFrom.Label, kv.Key, stateTo.Label));
                }
            }
            //Graph generating
            byte[] output = wrapper.GenerateGraph(sbFsm.ToString(), GraphVizWrapper.Enums.GraphReturnType.Png);

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

            return image;
        }
        
    }
}
