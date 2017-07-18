namespace PEG.SyntaxTree
{
    public class CharacterTerminal : Terminal
    {
        public char Character { get; }

        public CharacterTerminal(char character)
        {
            Character = character;
        }

        public override string ToString()
        {
            return "'" + Character + "'";
        }

        public override string Coalesce()
        {
            return Character.ToString();
        }

        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            if (Equals(engine.Current))
            {
                var position = engine.Position;
                if (engine.Consume())
                {
                    return OutputResult(engine.Output, position);
                }
                else
                {
                    return engine.False;
                }
            }
            return engine.False;
        }

        public bool Equals(CharacterTerminal other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Character == Character;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(CharacterTerminal)) return false;
            return Equals((CharacterTerminal)obj);
        }

        public override int GetHashCode()
        {
            return Character.GetHashCode();
        }

        public static implicit operator CharacterTerminal(char c)
        {
            return new CharacterTerminal(c);
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}