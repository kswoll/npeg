using System.Collections.Generic;
using PEG.Cst;

namespace PEG.SyntaxTree
{
    public abstract class Terminal : Expression, ICstTerminalNode
    {
        Terminal ICstTerminalNode.Terminal => this;

        public ParseOutputSpan OutputResult(ParseOutput output, int position)
        {
            output.Add(new OutputRecord(this, position));
            return new ParseOutputSpan(true, position, position + 1);
        }

        public abstract string Coalesce();

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}