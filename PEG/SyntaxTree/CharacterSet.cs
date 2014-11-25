using System.Collections.Generic;
using System.Linq;

namespace PEG.SyntaxTree
{
    public class CharacterSet : Expression
    {
        public IEnumerable<CharacterTerminal> Characters { get; set; }
        public CharacterTerminal[] CharacterTable { get; set; }

        public CharacterSet(params CharacterTerminal[] characters) : this((IEnumerable<CharacterTerminal>)characters)
        {
        }

        public CharacterSet(IEnumerable<CharacterTerminal> characters)
        {
            Characters = characters;

            CharacterTerminal[] orderedCharacters = characters.OrderBy(o => o.Character).ToArray();
            int maxCharacter = orderedCharacters.Last().Character;
            CharacterTable = new CharacterTerminal[maxCharacter + 1];
            foreach (CharacterTerminal c in orderedCharacters)
                CharacterTable[c.Character] = c;
        }

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            Terminal current = engine.Current;
            if (current is CharacterTerminal && ((CharacterTerminal)current).Character < CharacterTable.Length && CharacterTable[((CharacterTerminal)current).Character] != null)
                return current.Execute(engine);
            else
                return null;
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}