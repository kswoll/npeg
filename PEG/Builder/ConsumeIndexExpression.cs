using System;
using System.Collections.Generic;
using System.Linq;
using PEG.Cst;
using PEG.Extensions;

namespace PEG.Builder
{
    [Consume("index-expression")]
    public class ConsumeIndexExpression : ConsumeExpression
    {
        [Consume("expression")]
        public ConsumeExpression Target { get; set; }

        [Consume("number")]
        public int Index { get; set; }

        public override string ToString()
        {
            return Target.ToString() + '[' + Index + ']';
        }

        public override IEnumerable<ICstNode> Resolve(ICstNonterminalNode node)
        {
            if (Index < 1)
                throw new InvalidOperationException("Indices in consume attributes are 1-based: " + this);

            ConsumeReferenceExpression targetAsReference = (ConsumeReferenceExpression)Target;
            Func<ICstNonterminalNode, ICstNode> ret = cstNode => cstNode.Children.OfType<CstNonterminalNode>().Where(o => o.Nonterminal.Name == targetAsReference.NonTerminal).ElementAtOrDefault(Index - 1);
            IEnumerable<ICstNode> result = new ICstNode[0];

            if (targetAsReference.Target != null)
                foreach (ICstNonterminalNode child in targetAsReference.Target.Resolve(node))
                {
                    if (child != null)
                    {
                        ICstNode element = ret(child);
                        if (element != null)
                            result = result.Union(element);
                    }
                }
            else
            {
                ICstNode element = ret(node);
                if (element != null)
                    result = result.Union(element);
            }

            return result;
        }
    }
}