namespace PEG.SyntaxTree
{
    public class EmptyString : Expression
    {
        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            return engine.Nothing;
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}