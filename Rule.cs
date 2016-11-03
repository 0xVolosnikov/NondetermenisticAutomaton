using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NondeterministicAutomaton
{
    class Rule
    {
        public char Symbol;
        public State NextState;

        public Rule(char symbol, State state)
        {
            Symbol = symbol;
            NextState = state;
        }
    }
}
