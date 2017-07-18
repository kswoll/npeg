using System.Collections.Generic;
using System.Linq;
using PEG.Extensions;

namespace PEG.SyntaxTree
{
    public class Sequence : Expression
    {
        public List<Expression> Expressions { get; set; }

        public Sequence()
        {
            Expressions = new List<Expression>();
        }

        public Sequence(params Expression[] expressions) : this()
        {
            Expressions.AddRange(expressions);
        }

        public static implicit operator Sequence(string sequence)
        {
            return new Sequence(sequence.ToCharArray().Select(o => (Terminal)o).ToArray());
        }

        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            var mark = engine.Mark();

            foreach (Expression expression in Expressions)
            {
                var current = expression.Execute(engine);
                if (current.IsFailed)
                {
                    engine.Reset(mark);
                    return engine.False;
                }
            }

            return engine.Return(mark);
        }

        public override string ToString()
        {
            return "(" + Expressions.Delimit(" + ") + ")";
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}