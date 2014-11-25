using System.Collections.Generic;

namespace PEG.SyntaxTree
{
    public class ForeignNonterminal : Expression
    {
        public string Name { get; set; }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            Token token = engine.Current as Token;
            if (token != null)
            {
                if (token.TokenRule.Name == Name)
                {
                    return token.Execute(engine);
                }
            }
            return null;
        }

        public override string ToString()
        {
            return Name;
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}