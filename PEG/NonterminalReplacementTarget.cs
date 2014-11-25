using System;
using PEG.Cst;
using PEG.SyntaxTree;

namespace PEG
{
    public class NonterminalReplacementTarget : IReplacementTarget
    {
        private Nonterminal nonterminal;

        public NonterminalReplacementTarget(Nonterminal nonterminal)
        {
            this.nonterminal = nonterminal;
        }

        public ICstNode Replace(CstCache cache)
        {
            return cache[nonterminal][0];
        }
    }
}