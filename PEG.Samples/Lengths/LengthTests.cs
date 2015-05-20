using NUnit.Framework;

namespace PEG.Samples.Lengths
{
    [TestFixture]
    public class LengthTests
    {
        [Test]
        public void IntegerDecimal()
        {
            var s = "5.5'";
            var length = Length.Parse(s);
            Assert.AreEqual(5.5, length.Feet);
        }

        [Test]
        public void InchesInteger()
        {
            var s = "5\"";
            var length = Length.Parse(s);
            Assert.AreEqual(5, length.Inches);
        }

        [Test]
        public void FeetAndInchesInteger()
        {
            var s = "5' 4\"";
            var length = Length.Parse(s);
            Assert.AreEqual(5, length.Feet);
            Assert.AreEqual(4, length.Inches);
        }
    }
}