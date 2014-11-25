using System.Collections.Generic;
using System.Linq;

namespace PEG.SyntaxTree
{
    public class Repeat : Expression
    {
        public Expression Operand { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            IEnumerable<OutputRecord> result = NoResults;
            IEnumerable<OutputRecord> current;

            int count;
            for (count = 0; !engine.IsFailure(current = Operand.Execute(engine)); count++)
            {
                if (count > Max)
                    return null;

                result = result.Concat(current);                
            }

            if (count < Min)
                return null;

            return result;
        }

        public override string ToString()
        {
            return Operand.ToString() + ".Repeat(" + Min + ", " + Max + ")";
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }        
    }
}