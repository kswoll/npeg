namespace PEG.SyntaxTree
{
    public struct ParseOutputSpan
    {
        public bool IsFailed { get; }
        public int Start { get; }
        public int End { get; }

        public ParseOutputSpan(bool isFailed, int start, int end)
        {
            IsFailed = isFailed;
            Start = start;
            End = end;
        }
    }
}