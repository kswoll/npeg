using System;
using System.Collections.Generic;

namespace PEG.SyntaxTree
{
    internal class NullTerminal : Terminal
    {
        public static NullTerminal Instance = new NullTerminal();

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