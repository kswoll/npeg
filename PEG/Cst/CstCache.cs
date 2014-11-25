using System;
using System.Collections.Generic;
using PEG.SyntaxTree;

namespace PEG.Cst
{
    public class CstCache
    {
        public Dictionary<Nonterminal, List<CstNonterminalNode>> cache = new Dictionary<Nonterminal, List<CstNonterminalNode>>();

        public CstCache(CstNonterminalNode node)
        {
            ScanNonterminal(node);
        }

        public List<CstNonterminalNode> this[Nonterminal nonterminal]
        {
            get
            {
                List<CstNonterminalNode> list;
                cache.TryGetValue(nonterminal, out list);
                return list;
            }
        }

        private void Cache(CstNonterminalNode node)
        {
            List<CstNonterminalNode> list;
            if (!cache.TryGetValue(node.Nonterminal, out list))
            {
                list = new List<CstNonterminalNode>();
                cache[node.Nonterminal] = list;
            }
            list.Add(node);
        }

        private void ScanNonterminal(CstNonterminalNode node)
        {
            Cache(node);

            foreach (var child in node.Children)
            {
                if (child is CstNonterminalNode)
                    ScanNonterminal((CstNonterminalNode)child);
            }
        }
    }
}