using System.Collections.Generic;
using PEG.SyntaxTree;

namespace PEG
{
    public enum MemoStatus { Failed, Success }

    public struct MemoEntry
    {
        public ParseOutputSpan Answer { get; }
        public int Position { get; }
        public IReadOnlyList<OutputRecord> Output { get; }

        public MemoEntry(ParseOutputSpan answer, int position, IReadOnlyList<OutputRecord> output = null)
        {
            Answer = answer;
            Position = position;
            Output = output;
        }
    }
}