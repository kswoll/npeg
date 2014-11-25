using System;
using System.Collections.Generic;

namespace PEG.SyntaxTree
{
    internal class AnyCharacterTerminal : Terminal
    {
        public static AnyCharacterTerminal Instance = new AnyCharacterTerminal();

        public override IEnumerable<OutputRecord> Execute(ParseEngine engine)
        {
            throw new NotImplementedException();
        }

        public override string Coalesce()
        {
            throw new NotImplementedException();
        }
    }
}