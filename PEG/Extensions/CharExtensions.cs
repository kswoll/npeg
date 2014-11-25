using System.Collections.Generic;
using System.Linq;

namespace PEG.Extensions
{
    public static class CharExtensions
    {
        public static IEnumerable<char> RangeTo(this char startCharacter, char endCharacter)
        {
            if (startCharacter == '(' || endCharacter == ')')
            {
                foreach (char c in new[] { '-', '\'', '\\', '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '=', '+', '[', ']', '{', '}', '|', ';', ':', '\"', '<', '>', ',', '.', '/', '?' })
                    yield return c;
            }
            else if ((startCharacter == 'A' && endCharacter == 'z') || (startCharacter == 'a' && endCharacter == 'Z'))
            {
                foreach (char c in 'A'.RangeTo('Z').Concat('a'.RangeTo('z')))
                    yield return c;
            }
            else
            {
                for (char c = startCharacter; c <= endCharacter; c++)
                    yield return c;
            }
        }        
    }
}