using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.IO;
using System.Drawing;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Interfaces;
using GraphVizWrapper.Queries;

namespace Finite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OutputWindow _outputWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            string regex = txtInput.Text;
            DFABuilder dfaBuilder = new DFABuilder();
            dfaBuilder.buildDFA(regex);
            if (_outputWindow != null)
            {
                _outputWindow.Close();
            }
            if ((bool)radioImmediate.IsChecked)
            {
                _outputWindow = new OutputWindow(dfaBuilder, false);
            }
            else if ((bool)radioStep.IsChecked)
            {
                _outputWindow = new OutputWindow(dfaBuilder, true);
            }
            _outputWindow.Show();
        }
    }
}
