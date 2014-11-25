using System;
using PEG.SyntaxTree;

namespace PEG
{
    public enum OutputType { None, Begin, End }

    public class OutputRecord
    {
        public OutputType OutputType { get; set; }
        public Expression Expression { get; set; }
        public int Position { get; set;}

        public OutputRecord(Terminal expression, int position)
        {
            Expression = expression;
            Position = position;
        }

        public OutputRecord(Nonterminal expression, OutputType outputType, int position)
        {
            Expression = expression;
            OutputType = outputType;
            Position = position;
        }

        public override string ToString()
        {
            switch (OutputType)
            {
                case OutputType.Begin:
                    return "Begin " + ((Nonterminal)Expression).Name;
                case OutputType.End:
                    return "End " + ((Nonterminal)Expression).Name;
                case OutputType.None:
                    return Expression.ToString();
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}