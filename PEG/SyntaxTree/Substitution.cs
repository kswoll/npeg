using System;
using System.Collections.Generic;

namespace PEG.SyntaxTree
{
    public class Substitution : Expression
    {
        public string Name { get; set; }
        public Expression Target { get; set; }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            return Target.Execute(engine);
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}