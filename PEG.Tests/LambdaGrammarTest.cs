using NUnit.Framework;

namespace PEG.Tests
{
    [TestFixture]
    public class LambdaGrammarTest
    {
        [Test]
        public void Integer()
        {
            bool success = (+('0'.To('9'))).Match("12345");
            Assert.IsTrue(success);
        }

        [Test]
        public void Decimal()
        {
            var i = +('0'.To('9'));
            var d = i + ~('.' + i);

            Assert.IsTrue(d.Match("12345"));
            Assert.IsTrue(d.Match("1.2"));
        }
    }
}