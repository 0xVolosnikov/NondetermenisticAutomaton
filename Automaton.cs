using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NondeterministicAutomaton
{
    public class Automaton
    {
        private SynchronizedCollection<ChainBounds> listBounds = new SynchronizedCollection<ChainBounds>();
        private List<State> _states = new List<State>();
        private HashSet<State> currentEpsilonClosure = new HashSet<State>();


        public Automaton(string regularExpression)
        {
            ConfigureByRegex(regularExpression);
        }

        public void ConfigureByRegex(string regex)
        {
            _states.Clear();
            int curInd = regex.Length - 1;
            _states.Add(new State(new List<Rule>(), _states.Count));
            ParseRegex(regex);
        }

        private void ParseRegex(string regex)
        {
            List<Rule> newRule = new List<Rule>();
            _states = ConfigureByRevPolRegex(RegexRevPol(regex + "#"));
        }

        private string RegexRevPol(string regex)
        {
            string konk = "";
            for(int i = 0; i< regex.Length; i++)
            {
                char symbol = regex[i];
                if (symbol != ' ')
                {
                    konk += symbol;
                    if ((i > 0 && regex[i - 1] == '\'') || (symbol != '+' && symbol != '(' && symbol != ' ' && symbol != '\''))
                    {
                        if (i < regex.Length - 1 && regex[i + 1] != '+' && regex[i + 1] != ')' && regex[i + 1] != '*' && regex[i + 1] != ' ')
                            konk += '.';
                    }
                }

            }
            regex = konk;

            string backNotString = "";
            int curPriority = 0;
            Stack<char> stack = new Stack<char>();
            for(int i = 0; i < regex.Length; i++)
            {
                if (i > 0 && regex[i - 1] == '\'')
                    backNotString += regex[i];

                else
                switch (regex[i])
                {
                    case '+':
                        if (curPriority < 1)
                            stack.Push('+');
                        else
                        {
                                while (stack.Any() && (stack.Peek() == '+' || stack.Peek() == '*' || stack.Peek() == '.'))
                                {
                                    backNotString += stack.Pop();
                                }
                            stack.Push('+');
                        }
                        curPriority = 1;
                        break;
                    case '*':
                        if (curPriority < 3)
                            stack.Push('*');
                        else
                        {
                            while (stack.Any() && (stack.Peek() == '*'))
                            {
                                backNotString += stack.Pop();
                            }
                            stack.Push('*');
                        }
                        curPriority = 3;
                        break;
                    case '(':
                            stack.Push('(');
                        break;
                    case ')':
                            while (stack.Any() && (stack.Peek() !='('))
                            {
                                backNotString +=  stack.Pop();
                            }
                        stack.Pop();
                        break;
                    case '.':
                        if (curPriority < 2)
                            stack.Push('.');
                        else
                        {
                            while (stack.Any() && (stack.Peek() == '.' || stack.Peek() == '*'))
                            {
                                backNotString += stack.Pop();
                            }
                            stack.Push('.');
                        }
                        curPriority = 2;
                        break;
                    case ' ':
                        break;
                    default:
                        backNotString += regex[i];
                        break;
                }

            }

            while (stack.Any())
                backNotString += stack.Pop();

            return backNotString;
        }


        private List<State> ConfigureByRevPolRegex(string regex)
        {
            int countOfStages = 1;
            Stack<List<State>> stack = new Stack<List<State>>();
            List<State> construction;
            List<State> structSec;
            List<State> structFirst;

            State startState = new State(new List<Rule>(), countOfStages++);
            startState.Add(new Rule('e', startState ));

            for (int i = 0; i < regex.Length; i++)
            {
                if (i > 0 && regex[i - 1] == '\'')
                {
                        construction = new List<State>();
                        State nodeStart = new State(new List<Rule>(), countOfStages++);
                        State nodeEnd = new State(new List<Rule>(), countOfStages++);
                        nodeStart.Add(new Rule(regex[i], nodeEnd));
                        construction.Add(nodeStart);
                        construction.Add(nodeEnd);
                        stack.Push(construction);
                }
                else
                    switch (regex[i])
                    {
                        case '*':
                            construction = new List<State>();
                            structFirst = stack.Pop();

                            State nodeStz = structFirst.First();
                            State nodeEndz = structFirst.Last();


                            State startz2 = new State(new List<Rule>(), countOfStages++);

                            State startz1 = new State(new List<Rule>(), countOfStages++);
                            State endz1 = new State(new List<Rule>(), countOfStages++);

                            State endz2 = new State(new List<Rule>(), countOfStages++);

                            startz2.Add(new Rule('e', startz1));
                            startz1.Add(new Rule('e', nodeStz));

                            nodeEndz.Add(new Rule('e', endz1));
                            endz1.Add(new Rule('e', endz2));

                            startz2.Add(new Rule('e', endz2));

                            endz1.Add(new Rule('e', startz1));

                            construction.Add(startz2);
                            construction.Add(startz1);
                            construction.AddRange(structFirst);
                            construction.Add(endz1);
                            construction.Add(endz2);
                            stack.Push(construction);

                            break;


                        case '+':
                            construction = new List<State>();
                            structSec = stack.Pop();
                            structFirst = stack.Pop();

                            State nodeUpSt = structFirst.First();
                            State nodeUpEnd = structFirst.Last();

                            State nodeDownSt = structSec.First();
                            State nodeDownEnd = structSec.Last();


                            State start = new State(new List<Rule>(), countOfStages++);
                            State end = new State(new List<Rule>(), countOfStages++);

                            start.Add(new Rule('e', nodeUpSt));
                            start.Add(new Rule('e', nodeDownSt));

                            nodeUpEnd.Add(new Rule('e', end));
                            nodeDownEnd.Add(new Rule('e', end));

                            construction.Add(start);
                            construction.AddRange(structFirst);
                            construction.AddRange(structSec);
                            construction.Add(end);
                            stack.Push(construction);

                            break;
                        case '.':
                            construction = new List<State>();
                            structSec = stack.Pop();
                            structFirst = stack.Pop();

                            State nodeSec = structSec.First();
                            State nodeFirst = structFirst.Last();

                            nodeFirst.Add(new Rule('e', nodeSec));

                            construction.AddRange(structFirst);
                            construction.AddRange(structSec);
                            stack.Push(construction);

                            break;
                        case '\'':
                            break;
                        default:
                            construction = new List<State>();
                            State nodeStart = new State(new List<Rule>(), countOfStages++);
                            State nodeEnd = new State(new List<Rule>(), countOfStages++);
                            nodeStart.Add(new Rule(regex[i], nodeEnd));
                            construction.Add(nodeStart);
                            construction.Add(nodeEnd);
                            stack.Push(construction);
                            break;
                    }
            }

            structFirst = stack.Pop();
            structFirst.First().Add(new Rule('e', structFirst.First()));
            return structFirst;
        }

        public List<ChainBounds> processChain(string chain)
        {
            processSubstring(_states.First(), chain, 0);
            return listBounds.ToList();
        }


        private void processSubstring(State state, string chain, int position)
        {
            var nextState = new State(new List<Rule>(), -1);
            var currentStates = new HashSet<State>();
            var startEpsilonClosure = new HashSet<State>();
            var correctSymbol = false;
            var correctInp = false;
            var correctChain = false;

            currentStates.Add(state);

            currentEpsilonClosure.Clear();
            foreach (State st in currentStates)
            {
                FindEpsilonClosure(st);
            }
            currentStates.UnionWith(currentEpsilonClosure);
            startEpsilonClosure = currentStates;
            ChainBounds bounds = null;
            for (int i = position; i < chain.Length; i++)
            {


               if (currentStates.Equals(startEpsilonClosure))
                {
                    if (bounds != null && correctChain)
                    {                   
                      //  listBounds.Add(bounds);
                    }
                    //if (!correctSymbol)
                    bounds = null;
                    correctSymbol = false;
                    correctChain = false;
                }

                HashSet<State> nextStates = new HashSet<State>();
                correctInp = false;
                foreach (State st in currentStates)
                {
                    foreach (Rule rl in st.Rules.Where(x => (x.Symbol == chain[i]) || (x.Symbol == 'e' && x.NextState == st) ) )
                    {
                        nextStates.Add(rl.NextState);

                        if (rl.Symbol != 'e'&& (rl.Symbol == chain[i])) correctInp = true;

                        if (!correctSymbol && rl.Symbol != 'e')
                        {
                            if (bounds != null)
                            bounds = new ChainBounds(bounds.End, i);
                              else 
                            bounds = new ChainBounds(i, i);
                            correctSymbol = true;
                        }
                    }
                }

                currentStates = nextStates;
                currentEpsilonClosure.Clear();
                foreach (State st in currentStates)
                {
                    FindEpsilonClosure(st);
                }
                currentStates.UnionWith(currentEpsilonClosure);

                if (correctInp)
                {
                    if (currentStates.Any(x => x.Num == _states.Last().Num - 1))
                    {
                        bounds.End = i;
                        listBounds.Add(bounds);
                        bounds = new ChainBounds(i, i);
                        correctChain = true;
                        currentStates = startEpsilonClosure;
                    }
                }
                else
                {
                    //if (correctChain)
                    {
                        bounds = null;
                        correctChain = false;
                        correctSymbol = false;
                        currentStates = startEpsilonClosure;
                    }
                }
            }

        }

        private void FindEpsilonClosure(State state)
        {
            foreach ( Rule rl in state.Rules.Where( x => x.Symbol == 'e' && !currentEpsilonClosure.Contains(x.NextState) ) )
            {
                currentEpsilonClosure.Add(rl.NextState);
                FindEpsilonClosure(rl.NextState);
            }    
        }

    }
}
