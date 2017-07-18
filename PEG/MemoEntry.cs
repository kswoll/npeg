using PEG.SyntaxTree;

namespace PEG
{
    public enum MemoStatus { Failed, Success }

    public struct MemoEntry
    {
        public ParseOutputSpan Answer { get; }
        public int Position { get; }

        public MemoEntry(ParseOutputSpan output, int position)
        {
            Answer = output;
            Position = position;
        }
    }
}