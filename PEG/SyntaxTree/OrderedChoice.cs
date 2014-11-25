using System.Collections.Generic;
using PEG.Extensions;

namespace PEG.SyntaxTree
{
    public class OrderedChoice : Expression
    {
        public List<Expression> Expressions { get; set; }

        public OrderedChoice()
        {
            Expressions = new List<Expression>();
        }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            IEnumerable<OutputRecord> nullResult = null;
            foreach (Expression expression in Expressions)
            {
                var current = expression.Execute(engine);
                if (!engine.IsFailure(current))
                    return current;
                else if (nullResult == null && current != null)
                    nullResult = current;
            }
            return nullResult;
        }

        public override string ToString()
        {
            return "(" + Expressions.Delimit(" | ") + ")";
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}