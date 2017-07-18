namespace PEG.SyntaxTree
{
    public class AndPredicate : Expression
    {
        public Expression Operand { get; set; }

        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            var mark = engine.Mark();
            var result = Operand.Execute(engine);
            engine.Reset(mark);
            return !result.IsFailed ? engine.Nothing : engine.False;
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }

        public override string ToString()
        {
            return "And(" + Operand.ToString() + ")";
        }
    }
}