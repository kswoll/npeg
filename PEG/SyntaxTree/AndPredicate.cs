using System.Collections.Generic;

namespace PEG.SyntaxTree
{
    public class AndPredicate : Expression
    {
        public Expression Operand { get; set; }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            var mark = engine.Mark();
            IEnumerable<OutputRecord> result = Operand.Execute(engine);
            engine.Reset(mark);
            return !engine.IsFailure(result) ? NoResults : null;
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }

        public override string ToString()
        {
            return "And(" + Operand.ToString() + ")";
        }
    }
}