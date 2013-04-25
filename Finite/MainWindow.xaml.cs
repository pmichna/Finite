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
            String msg = regex.Value + "\n" +
                "IsUnion: " + regex.IsUnion + "\n" +
                "IsConcatenation: " + regex.IsConcatenation + "\n" +
                "IsPlus: " + regex.IsPlus + "\n" +
                "IsKleene: " + regex.IsKleene + "\n" +
                "IsEmptySet: " + regex.IsEmptySet + "\n" +
                "IsEmptyWord: " + regex.IsEmptyWord + "\n";
            RegularExpression r, s;
            if (regex.IsConcatenation)
            {
                regex.GetConcatSubExpressions(out r, out s);
                msg += "Concat subexpressions: " + r.Value + "___" + s.Value + "\n";
            }
            if (regex.IsUnion)
            {
                regex.GetUnionSubExpressions(out r, out s);
                msg += "Union subexpressions: " + r.Value + "___" + s.Value + "\n";
            }
            MessageBox.Show(msg);
        }
    }
}
