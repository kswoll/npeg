using System.Collections.Generic;

namespace PEG.SyntaxTree
{
    public class CharacterRange : CharacterSet
    {
        public char StartCharacter { get; set; }
        public char EndCharacter { get; set; }

        public CharacterRange(char startCharacter, char endCharacter, params CharacterTerminal[] characters) : base(characters)
        {
            StartCharacter = startCharacter;
            EndCharacter = endCharacter;
        }

        public CharacterRange(char startCharacter, char endCharacter, IEnumerable<CharacterTerminal> characters) : base(characters)
        {
            StartCharacter = startCharacter;
            EndCharacter = endCharacter;
        }

        public override string ToString()
        {
            return string.Format("'{0}'.To('{1}')", StartCharacter, EndCharacter);
        }
    }
}