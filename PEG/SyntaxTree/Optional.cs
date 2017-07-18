namespace PEG.SyntaxTree
{
    public class Optional : Expression
    {
        public Expression Operand { get; set; }

        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            var result = Operand.Execute(engine);
            return !result.IsFailed ? result : engine.Nothing;
        }

        public override string ToString()
        {
            return "~" + Operand.ToString();
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}