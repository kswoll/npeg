using System.Collections.Generic;
using PEG.Cst;

namespace PEG.Builder
{
    public class ConsumeConditional : ConsumeExpression
    {
        [Consume("expression[1]")]
        public ConsumeExpression Predicate { get; set; }

        [Consume("expression[2]")]
        public ConsumeExpression Expression { get; set; }

        public override IEnumerable<ICstNode> Resolve(ICstNonterminalNode node)
        {
            if (Predicate == null || Predicate.Resolve(node) != null)
                return Expression.Resolve(node);
            else
                return new ICstNode[0];
        }
    }
}