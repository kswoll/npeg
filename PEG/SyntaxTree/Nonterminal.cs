namespace PEG.SyntaxTree
{
    public class Nonterminal : Expression
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public Expression Expression { get; set; }

        // When used as tokens
        public bool IsToken { get; set; }
        public bool IsTokenOmitted { get; set; }

        public Nonterminal()
        {
        }

        public Nonterminal(Expression expression)
        {
            Expression = expression;
        }

        public ParseOutputSpan Eval(ParseEngine engine)
        {
            var mark = engine.Mark();
            engine.Output.Add(new OutputRecord(this, OutputType.Begin, mark.OutputPosition));

            var children = Expression.Execute(engine);
            if (children.IsFailed)
            {
                engine.Reset(mark);
                return engine.False;
            }
            else
            {
                engine.Output.Add(new OutputRecord(this, OutputType.End, engine.Position));
                return engine.Return(mark);
            }
        }

        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            engine.Log(ToString());
            engine.Log("{");

            var nonterminal = engine.ApplyNonterminal(this, engine.Position);
            if (nonterminal.IsFailed)
                engine.Log(ParseEngine.Indent + "Failed");
            else
                engine.Log(ParseEngine.Indent + "Succeeded");

            engine.Log("}");

            return nonterminal;
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