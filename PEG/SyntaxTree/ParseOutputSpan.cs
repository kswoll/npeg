using System.Collections.Generic;

namespace PEG.SyntaxTree
{
    public struct ParseOutputSpan
    {
        public bool IsFailed { get; }
        public bool IsLeftRecursion { get; }
        public int Start { get; }
        public int End { get; }
        public bool Any => Count > 0;
        public int Count => End - Start;

        public ParseOutputSpan(bool isLeftRecursion) : this()
        {
            IsFailed = true;
            IsLeftRecursion = isLeftRecursion;
            Start = -1;
            End = -1;
        }

        public ParseOutputSpan(bool isFailed, int start, int end)
        {
            IsFailed = isFailed;
            IsLeftRecursion = false;
            Start = start;
            End = end;
        }

        public IEnumerable<OutputRecord> GetRecords(ParseOutput output)
        {
            return output.GetRecords(this);
        }
    }
}