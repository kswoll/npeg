using System;
using PEG.SyntaxTree;

namespace PEG
{
    public enum OutputType { None, Begin, End }

    public struct OutputRecord
    {
        public OutputType OutputType { get; }
        public Expression Expression { get; }
        public int Position { get; }

        public OutputRecord(Terminal expression, int position)
        {
            OutputType = OutputType.None;
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