using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PEG.SyntaxTree;

namespace PEG.Cst
{
    public class CstNonterminalNode : ICstNonterminalNode
    {
        public int AbsoluteIndex { get; private set; }
        public Nonterminal Nonterminal { get; private set; }
        public List<ICstNode> Children { get; private set; }

        public CstNonterminalNode(Nonterminal nonterminal, int absoluteIndex)
        {
            Nonterminal = nonterminal;
            Children = new List<ICstNode>();
            AbsoluteIndex = absoluteIndex;
        }

        public ICstNode Transform(Func<CstNonterminalNode, ICstNode> transformer)
        {
            ICstNode result = transformer(this);
            if (result == null)
            {
                result = new CstNonterminalNode(Nonterminal, -1);

                foreach (var child in Children)
                {
                    if (child is CstNonterminalNode)
                    {
                        var nonterminalChild = (CstNonterminalNode)child;
                        ((CstNonterminalNode)result).Children.Add(nonterminalChild.Transform(transformer));
                    }
                    else
                        ((CstNonterminalNode)result).Children.Add(child);
                }
            }
            return result;
        }

        public override string ToString()
        {
            return Nonterminal != null ? Nonterminal.Name : "Root";
        }

        public string Coalesce()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ICstNode child in Children)
            {
                builder.Append(child.Coalesce());
            }
            return builder.ToString();
        }

        public IEnumerable<ICstNonterminalNode> FindAllNonterminalNodes()
        {
            Stack<ICstNode> nodes = new Stack<ICstNode>();
            nodes.Push(this);

            while (nodes.Any())
            {
                var node = nodes.Pop();
                var nonterminalNode = node as ICstNonterminalNode;

                if (nonterminalNode != null)
                {
                    yield return nonterminalNode;

                    foreach (var child in nonterminalNode.Children)
                        nodes.Push(child);
                }
            }
        }
    }
}