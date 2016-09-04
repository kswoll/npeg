using System;
using System.Collections.Generic;
using System.Linq;

namespace PEG.SyntaxTree
{
    public class EncloseExpression : Expression
    {
        public Expression Enclosure { get; set; }
        public Expression Operand { get; set; }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            var current = Enclosure.Execute(engine);
            if (engine.IsFailure(current))
                return current;

            Func<IEnumerable<OutputRecord>> predicate = () =>
            {
                var mark = engine.Mark();
                var result = Enclosure.Execute(engine);
                engine.Reset(mark);
                return result;
            };

            engine.AddInterceptor(predicate);
            try
            {
                var operandResult = Operand.Execute(engine);
                if (engine.IsFailure(operandResult))
                    return operandResult;

                current = current.Concat(operandResult);
            }
            finally
            {
                engine.RemoveInterceptor(predicate);
            }

            var closeResult = Enclosure.Execute(engine);
            if (engine.IsFailure(closeResult))
                return closeResult;

            return current.Concat(closeResult);
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}