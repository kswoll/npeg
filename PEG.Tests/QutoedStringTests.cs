using System;
using NUnit.Framework;

namespace PEG.Tests
{
    [TestFixture]
    public class QutoedStringTests
    {
        [Test]
        public void EscapedQuote()
        {
            var body = (-(!('"'._() | @"\") + Peg.Any | @"\\" | @"\""")).Capture();
            var pattern = '"' + body + '"';

            Console.WriteLine(pattern.Parse("\"abc\"")[body]);
            Console.WriteLine(pattern.Parse("\"a\\\"bc\"")[body]);

        }
    }
}