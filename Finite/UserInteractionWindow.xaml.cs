using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Finite
{
    /// <summary>
    /// Interaction logic for MyDialog.xaml
    /// </summary>
    public partial class UserInteraction : Window
    {
        private List<State> _states;
        private State _selectedState;

        public UserInteraction(string test, List<State> states)
        {
            
            InitializeComponent();
            lbl.Content = "Something equivalent to " + test + "? (select from the list and click \"OK\")";
            _states = states;
            lstStates.ItemsSource = _states;
        }

        private void lstStates_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedState = (State) lstStates.SelectedItem;
        }

        public State GetSelectedState()
        {
            return _selectedState;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedState != null)
                Close();
            else
            {
                MessageBox.Show("You have to choose the equivalent state first or click \"Noting equivalent\".");
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
