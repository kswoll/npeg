using System.Collections;
using System.Collections.Generic;
using PEG.SyntaxTree;

namespace PEG
{
    public class StringParseInput : IParseInput
    {
        private string data;
        private ParseEngine engine;

        public StringParseInput(ParseEngine engine, string data)
        {
            this.engine = engine;
            this.data = data;
        }

        public Terminal this[int index]
        {
            get
            {
                Terminal result = engine.TerminalsCache[data[index]];
                if (result == null)
                {
                    result = new CharacterTerminal(data[index]);
                    engine.TerminalsCache[data[index]] = result;
                }
                return result;
            }
        }

        public int Length
        {
            get { return data.Length; }
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