using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finite
{
    public class State
    {
        private Dictionary<State, char> transistions = new Dictionary<State, char>();
        public bool IsFinal { get; set; }
    }
}
