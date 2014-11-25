using System.Collections.Generic;
using PEG.Cst;

namespace PEG.SyntaxTree
{
    public abstract class Terminal : Expression, ICstTerminalNode
    {
        Terminal ICstTerminalNode.Terminal
        {
            get { return this; }
        }

        public IEnumerable<OutputRecord> AsResult(int position)
        {
            yield return new OutputRecord(this, position);
        }

        public abstract string Coalesce();

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}