using System.Collections.Generic;

namespace PEG
{
    public enum MemoStatus { Failed, Success }

    public class MemoEntry
    {
        public IEnumerable<OutputRecord> Answer { get; set; }
        public int Position { get; set; }

        public MemoEntry(IEnumerable<OutputRecord> output, int position)
        {
            Answer = output;
            Position = position;
        }
    }
}