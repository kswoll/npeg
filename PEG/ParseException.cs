using System;

namespace PEG
{
    public class ParseException : Exception
    {
        public ParseException(string input, int position) : base(new ParseError(input, position).ToString())
        {
        }
    }
}