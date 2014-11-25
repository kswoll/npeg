using System.Collections.Generic;

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

        public IEnumerable<OutputRecord> AsResult(IEnumerable<OutputRecord> children, int startPosition, int endPosition)
        {
            yield return new OutputRecord(this, OutputType.Begin, startPosition);
            foreach (OutputRecord child in children)
                yield return child;
            yield return new OutputRecord(this, OutputType.End, endPosition);
        }

        public IEnumerable<OutputRecord> Eval(ParseEngine context)
        {
            int startPosition = context.Position;
            IEnumerable<OutputRecord> children = Expression.Execute(context);
            if (context.IsFailure(children))
                return children;
            else
            {
                int endPosition = context.Position;
                return AsResult(children, startPosition, endPosition);
            }
        }

        public override IEnumerable<OutputRecord> Execute(ParseEngine context)
        {
            context.Log(ToString());
            context.Log("{");
//            if (!context.Grammar.IsTokenGrammar)
//                Console.WriteLine("Executing " + Name);
            IEnumerable<OutputRecord> nonterminal = context.ApplyNonterminal(this, context.Position);
//            if (!context.Grammar.IsTokenGrammar)
//                Console.WriteLine("Executed " + Name);
//            if (!context.IsFailure(nonterminal) && !context.Grammar.IsTokenGrammar)
//                Logger.WriteLine("Accepted " + Name + " (" + nonterminal.Concatenate(" ", o => o.OutputType == OutputType.None ? o.ToString() : "") + ")");

            if (context.IsFailure(nonterminal))
                context.Log(ParseEngine.Indent + "Failed");
            else 
                context.Log(ParseEngine.Indent + "Succeeded");

            context.Log("}");

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