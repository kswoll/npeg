using System.Collections;
using System.Collections.Generic;
using PEG.SyntaxTree;

namespace PEG
{
    public class TokenParseInput : IParseInput
    {
        private Terminal[] input;

        public TokenParseInput(Terminal[] input)
        {
            this.input = input;
        }

        public Terminal this[int index]
        {
            get { return input[index]; }
        }

        public int Length
        {
            get { return input.Length; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Terminal> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
                yield return this[i];
        }
    }
}