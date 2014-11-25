using System.Collections.Generic;

namespace PEG.SyntaxTree
{
    public class NotPredicate : Expression
    {
        public Expression Operand { get; set; }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            var mark = engine.Mark();
            IEnumerable<OutputRecord> result = Operand.Execute(engine);
            IEnumerable<OutputRecord> returnValue;
            if (engine.IsFailure(result))
                returnValue = NoResults;
            else
                returnValue = null;
            engine.Reset(mark);
            return returnValue;
        }

        public override string ToString()
        {
            return "!" + Operand.ToString();
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}