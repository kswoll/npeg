using System.Collections.Generic;
using System.Linq;
using PEG.Cst;

namespace PEG.Builder
{
    public class ConsumeChoice : ConsumeExpression
    {
        public List<ConsumeExpression> Choices { get; set; }

        public ConsumeChoice()
        {
            Choices = new List<ConsumeExpression>();
        }

        public override IEnumerable<ICstNode> Resolve(ICstNonterminalNode node)
        {
            IEnumerable<ICstNode> result = new ICstNode[0];
            foreach (ConsumeExpression choice in Choices)
            {
                result = result.Concat(choice.Resolve(node));
            }
            bool anyNull = result.Contains(null);
            return result.Cast<ICstNonterminalNode>().OrderBy(o => o.AbsoluteIndex).Cast<ICstNode>();
        }
    }
}