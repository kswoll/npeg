namespace PEG.SyntaxTree
{
    public class NotPredicate : Expression
    {
        public Expression Operand { get; set; }

        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            var mark = engine.Mark();
            var result = Operand.Execute(engine);
            var returnValue = result.IsFailed ? engine.Nothing : engine.False;
            engine.Reset(mark);
            return returnValue;
        }

        public override string ToString()
        {
            return "!" + Operand.ToString();
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}