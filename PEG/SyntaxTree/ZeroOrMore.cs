using System.Collections.Generic;
using System.Linq;

namespace PEG.SyntaxTree
{
    public class ZeroOrMore : Expression
    {
        public Expression Operand { get; set; }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            IEnumerable<OutputRecord> result = NoResults;
            IEnumerable<OutputRecord> current;
            while (!engine.IsFailure(current = Operand.Execute(engine))) 
                result = result.Concat(current);
            return result;
        }

        public override string ToString()
        {
            return "-" + Operand.ToString();
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}