using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NondeterministicAutomaton
{
    public class ChainBounds
    {
        public int Start;
        public int End;

        public ChainBounds(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}
