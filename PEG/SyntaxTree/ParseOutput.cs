using System.Collections.Generic;

namespace PEG.SyntaxTree
{
    public class ParseOutput
    {
        public int Count => count;

        private readonly List<OutputRecord> storage = new List<OutputRecord>();

        private int count;

        public void Add(OutputRecord record)
        {
            if (count < storage.Count)
            {
                storage[count] = record;
            }
            else
            {
                storage.Add(record);
            }
            count++;
        }

        public int Mark()
        {
            return count;
        }

        public void Reset(int outputPosition)
        {
            count = outputPosition;
        }

        public IEnumerable<OutputRecord> GetRecords(ParseOutputSpan parseOutputSpan)
        {
            for (var i = parseOutputSpan.Start; i < parseOutputSpan.End; i++)
            {
                yield return storage[i];
            }
        }
    }
}