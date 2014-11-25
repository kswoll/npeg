using System.Collections.Generic;
using PEG.SyntaxTree;
using PEG.Utils;

namespace PEG
{
    public class LeftRecursion
    {
        public Nonterminal Rule { get; set; }
        public BooleanSet InvolvedSet { get; set; }
        public BooleanSet EvalSet { get; set; }
        public LeftRecursion Next { get; set; }

        public LeftRecursion(Nonterminal rule, Grammar grammar)
        {
            Rule = rule;
            int size = grammar.Nonterminals.Count;
            InvolvedSet = new BooleanSet(size);
            EvalSet = new BooleanSet(size);
        }

        public LeftRecursion(Nonterminal rule, LeftRecursion next, Grammar grammar) 
        {
            Rule = rule;
            InvolvedSet = new BooleanSet(grammar.Nonterminals.Count);
            Next = next;
            EvalSet = next.EvalSet.Copy();
            EvalSet.Add(next.Rule.Index);
        }

        public void Add(IEnumerable<Nonterminal> involvedSet)
        {
            foreach (Nonterminal nonterminal in involvedSet)
            {
                InvolvedSet.Add(nonterminal.Index);
            }
        }
    }
}