using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NondeterministicAutomaton
{
    class State
    {
        public List<Rule> Rules;
        public int Num;
        public State(List<Rule> rules, int num)
        {
            Rules = rules;
            Num = num;
        }
        public void Add(Rule rule)
        {
            Rules.Add(rule);
        }
        public void AddRange(List<Rule> rules)
        {
            Rules.AddRange(rules);
        }
    }
}
