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

        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            if (engine.Current == null)
                return engine.False;
            if (OutputCharacters)
                return engine.Current.Execute(engine);
            else
            {
                var mark = engine.Output.Mark();
                if (!engine.Current.Execute(engine).IsFailed)
                {
                    engine.Output.Reset(mark);
                    return engine.Nothing;
                }
            }
            return engine.False;
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }

        public override string ToString()
        {
            return "Any";
        }
    }
}