namespace PEG.SyntaxTree
{
    public class OneOrMore : Expression
    {
        public Expression Operand { get; set; }

        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            var mark = engine.Mark();
            var first = Operand.Execute(engine);
            if (first.IsFailed)
            {
                return engine.False;
            }

            while (!Operand.Execute(engine).IsFailed)
            {
                // Execute mutates state, so this body doesn't need to do anything
            }

            return engine.Return(mark);
        }

        public override string ToString()
        {
            return "+" + Operand.ToString();
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}