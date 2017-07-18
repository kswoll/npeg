namespace PEG.SyntaxTree
{
    public class Repeat : Expression
    {
        public Expression Operand { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            var mark = engine.Mark();
            int count;
            for (count = 0; !Operand.Execute(engine).IsFailed; count++)
            {
                if (count > Max)
                    return engine.Nothing;
            }

            if (count < Min)
                return engine.Nothing;

            return engine.Return(mark);
        }

        public override string ToString()
        {
            return Operand.ToString() + ".Repeat(" + Min + ", " + Max + ")";
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}