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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Finite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            RegularExpression regex = new RegularExpression(txtInput.Text);
            //String msg = regex.Value + "\n" +
            //    "IsUnion: " + regex.IsUnion + "\n" +
            //    "IsConcatenation: " + regex.IsConcatenation + "\n" +
            //    "IsPlus: " + regex.IsPlus + "\n" +
            //    "IsKleene: " + regex.IsKleene + "\n" +
            //    "IsEmptySet: " + regex.IsEmptySet + "\n" +
            //    "IsEmptyWord: " + regex.IsEmptyWord + "\n";
            //RegularExpression r, s;
            //if (regex.IsConcatenation)
            //{
            //    regex.GetConcatSubExpressions(out r, out s);
            //    msg += "Concat subexpressions: " + r.Value + "___" + s.Value + "\n";
            //}
            //if (regex.IsUnion)
            //{
            //    regex.GetUnionSubExpressions(out r, out s);
            //    msg += "Union subexpressions: " + r.Value + "___" + s.Value + "\n";
            //}
            //MessageBox.Show(msg);
            //char a = txtChar.Text[0];
            //string msg = DFABuilder.Derive(regex, a).Value;
            //MessageBox.Show(msg);
            DFA dfa = DFABuilder.buildDFA(txtInput.Text);
            string msg = "DFA built! \nInitial state: " + dfa.InitState.Label + "\n\n" +
                "States:\n";
            foreach (State s in dfa.States)
                msg += s.Label + "\n";
            msg += "\nTransistions:\n";
            foreach (State s in dfa.States)
            {
                foreach(KeyValuePair<char, State> trans in s.transistions)
                {
                    msg += "From: " + s.Label + " to: " + trans.Value.Label + " over: " + trans.Key + "\n";
                }
            }
            MessageBox.Show(msg);
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            RegularExpression regex = new RegularExpression(txtInput.Text);
            DFA dfa = DFABuilder.buildDFA(txtInput.Text);
            //string msg = "DFA built! \nInitial state: " + dfa.InitState.Label + "\n\n" +
            //    "States:\n";
            //foreach (State s in dfa.States)
            //    msg += s.Label + "\n";
            //msg += "\nFinal states:\n";
            //foreach (State s in dfa.FinalStates)
            //    msg += s.Label + "\n";
            //msg += "\nTransistions:\n";
            //foreach (State s in dfa.States)
            //{
            //    foreach (KeyValuePair<char, State> trans in s.transistions)
            //    {
            //        msg += "From: " + s.Label + " to: " + trans.Value.Label + " over: " + trans.Key + "\n";
            //    }
            //}
            //MessageBox.Show(msg);
            if ((bool)radioImmediate.IsChecked)
            {
                var outputWindow = new OutputWindow(dfa);
                outputWindow.Show();
            }
            else if ((bool)radioStep.IsChecked)
            {
                var outputStepWindow = new OutputStepWindow(dfa);
                outputStepWindow.Show();
            }
        }
    }
}
