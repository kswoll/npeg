using NUnit.Framework;
using PEG.SyntaxTree;

namespace PEG.Tests
{
    [TestFixture]
    public class EncloseTest
    {
        [Test]
        public void SimpleQuotes()
        {
            Assert.IsTrue('"'._().Enclose(-'*'.Any()).Match("\"Hello\""));
        }
    }
}