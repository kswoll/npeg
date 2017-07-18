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

        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            foreach (var expression in Expressions)
            {
                var current = expression.Execute(engine);
                if (!current.IsFailed)
                    return current;
            }
            return engine.False;
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