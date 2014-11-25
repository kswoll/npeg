using System.Collections.Generic;
using System.Linq;
using PEG.Cst;

namespace PEG.Builder
{
    [Consume("expression")]
    public abstract class ConsumeExpression
    {
        public abstract IEnumerable<ICstNode> Resolve(ICstNonterminalNode node);

        public string ResolveAsString(ICstNonterminalNode node)
        {
            var result = Resolve(node);
            if (result.Any())
                return result.First().Coalesce();
            else
                return null;
        }
    }
}