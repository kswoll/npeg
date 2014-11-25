using System.Collections.Generic;

namespace PEG.SyntaxTree
{
    public class Optional : Expression
    {
        public Expression Operand { get; set; }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            IEnumerable<OutputRecord> result = Operand.Execute(engine);
            if (!engine.IsFailure(result))
                return result;
            return NoResults;
        }

        public override string ToString()
        {
            return "~" + Operand.ToString();
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}