using System.Collections.Generic;
using System.Linq;

namespace PEG.SyntaxTree
{
    public class AnyCharacter : Expression
    {
        public bool OutputCharacters { get; set; }

        public AnyCharacter()
        {
            OutputCharacters = true;
        }

        public AnyCharacter(bool outputCharacters)
        {
            OutputCharacters = outputCharacters;
        }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            if (engine.Current == null)
                return null;
            if (OutputCharacters)
                return engine.Current.Execute(engine);
            else if (engine.Current.Execute(engine).Any())
                return NoResults;
            return null;
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}