using System.Collections.Generic;

namespace PEG.SyntaxTree
{
    public class EmptyString : Expression
    {
        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            return NoResults;
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}