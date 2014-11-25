using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PEG.Cst;

namespace PEG.Builder
{
    [Consume("reference-expression")]
    public class ConsumeReferenceExpression : ConsumeExpression
    {
        [Consume("expression")]
        public ConsumeExpression Target { get; set; }

        [Consume("identifier")]
        public string NonTerminal { get; set; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            if (Target != null)
            {
                result.Append(Target);
                result.Append('.');
            }
            result.Append(NonTerminal);
            return result.ToString();
        }

        public override IEnumerable<ICstNode> Resolve(ICstNonterminalNode node)
        {
            Func<ICstNonterminalNode, IEnumerable<ICstNode>> ret = cstNode => cstNode.Children.OfType<ICstNonterminalNode>().Where(o => o.Nonterminal.Name == NonTerminal).Cast<ICstNode>();
            IEnumerable<ICstNode> result = new ICstNode[0];
            if (Target != null)
            {
                foreach (ICstNonterminalNode current in Target.Resolve(node))
                {
                    result = result.Concat(ret(current));
                }
            }
            else
            {
                result = result.Concat(ret(node));
            }
            return result;
        }
    }
}