using System.Collections.Generic;
using System.Linq;

namespace PEG.SyntaxTree
{
    public class OneOrMore : Expression
    {
        public Expression Operand { get; set; }

        public IEnumerable<IEnumerable<OutputRecord>> GetResults(ParseEngine engine, IEnumerable<OutputRecord> first)
        {
            do
            {
                yield return first;
            }
            while (!engine.IsFailure(first = Operand.Execute(engine)));
        }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            IEnumerable<OutputRecord> first = Operand.Execute(engine);
            if (engine.IsFailure(first))
                return first;
            else
                return GetResults(engine, first).ToArray().SelectMany(o => o);
        }

        public override string ToString()
        {
            return "+" + Operand.ToString();
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}