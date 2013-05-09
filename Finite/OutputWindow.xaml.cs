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
using QuickGraph;

namespace Finite
{
    /// <summary>
    /// Interaction logic for OutputWindow.xaml
    /// </summary>
    public partial class OutputWindow : Window
    {
        private IBidirectionalGraph<object, IEdge<object>> _graphToVizualize;
        private DFA _dfa;

        public IBidirectionalGraph<object, IEdge<object>> GraphToVizualize
        {
            get { return _graphToVizualize; }
        }

        public OutputWindow(DFA dfa)
        {
            _dfa = dfa;
            CreateGraphToVizualize();
            
            InitializeComponent();
            
        }

        private void CreateGraphToVizualize()
        {
            var g = new BidirectionalGraph<object, IEdge<object>>(true);
           // var g1 = new BidirectionalGraph<object, IEdge<object>>(true);

            //add vertices
            //string[] vertices = new string[5];
            //for (int i = 0; i < 5; i++)
            //{
            //    vertices[i] = i.ToString();
            //    g.AddVertex(vertices[i]);
            //}
            foreach (State s in _dfa.States)
            {
                g.AddVertex(s.ToString());
            }

            //add edges
            foreach (State s in _dfa.States)
            {
                foreach (KeyValuePair<char, State> t in s.transistions)
                {
                    g.AddEdge(new Edge<object>(s.ToString(), t.Value.ToString()));
                }
            }

            //g.AddEdge(new Edge<object>(vertices[0], vertices[1]));
            //g.AddEdge(new Edge<object>(vertices[1], vertices[2]));
            //g.AddEdge(new Edge<object>(vertices[2], vertices[3]));
            //g.AddEdge(new Edge<object>(vertices[3], vertices[1]));
            //g.AddEdge(new Edge<object>(vertices[1], vertices[4]));
            //g.AddEdge(new Edge<object>(vertices[1], vertices[4]));
            //g.AddEdge(new Edge<object>(vertices[4], vertices[4]));

            _graphToVizualize = g;

        }
    }
}
