using NUnit.Framework;
using static PEG.Peg;

namespace PEG.Tests
{
    [TestFixture]
    public class EncloseTest
    {
        [Test]
        public void SimpleQuotes()
        {
            Assert.IsTrue('"'.Enclose(-Any).Match("\"Hello\""));
        }
    }
}