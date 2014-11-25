using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace PEG.Tests
{
    [TestFixture]
    public class ExpressionExtensionsTest
    {
        [Test]
        public void MatchString()
        {
            var pattern = +('a'.To('Z') | '.') + '@' + +('a'.To('Z') | '.');

            Assert.IsTrue(pattern.Match("john@test.com"));
            Assert.IsFalse(pattern.Match("john@test@com"));
        }

        [Test]
        public void ContainsString()
        {
            var pattern = +('a'.To('Z') | '.') + '@' + +('a'.To('Z') | '.');

            var pattern2 = +'0'.To('9');
            pattern2.Match("42");

            Assert.IsTrue(pattern.Contains("asdf john@test.com asfd asdf asdf asdf"));
            Assert.IsFalse(pattern.Contains("asdf john%test%com"));            
        }

        [Test]
        public void Test()
        {
            var pattern = '"' + +(!('"'._() | @"\") + Peg.Any | @"\\" | @"\""") + '"';

            Assert.IsTrue(pattern.Match("\"value\""));
            Assert.IsTrue(pattern.Match("\"the \\\"quote\\\"\""));
        }

        /// <summary>
        /// Replace the month and day components.
        /// </summary>
        [Test]
        public void Replace()
        {
            var digit = ('0').To('9');
            var month = digit.Repeat(1, 2).Capture();
            var day = digit.Repeat(1, 2).Capture();
            var pattern = month + '/' + day + '/' + digit.Repeat(4);

            const string input = "12/1/2004";
            string result = pattern.Transform(input, month.To(day), day.To(month));

            Assert.AreEqual("1/12/2004", result);
        }

        /// <summary>
        /// Replace the month and day components.
        /// </summary>
        [Test]
        public void ReplaceAll()
        {
            var digit = ('0').To('9');
            var month = digit.Repeat(1, 2).Capture();
            var day = digit.Repeat(1, 2).Capture();
            var pattern = month + '/' + day + '/' + digit.Repeat(4);

            const string input = "12/1/2004 and 2/8/1988";
            
            string result = pattern.Replace(input, month.To(day), day.To(month));

            Assert.AreEqual("1/12/2004 and 8/2/1988", result);
        }

        [Test]
        public void ReplaceToString()
        {
            var digit = ('0').To('9');
            var month = digit.Repeat(1, 2).Capture();
            var day = digit.Repeat(1, 2).Capture();
            var pattern = month + '/' + day + '/' + digit.Repeat(4);

            const string input = "12/1/2004 and 2/8/1988";

            string result = pattern.Replace(input, month.To("MM"));

            Assert.AreEqual("MM/1/2004 and MM/8/1988", result);
        }

        [Test]
        public void Length()
        {
            var number = +'0'.To('9');
            var feet = number.Capture();
            var inches = number.Capture();
            var pattern = feet + '\'' + ~(-' '._() + inches + '\"');

            var results = pattern.Parse("5' 6\"");
            int feetResult = int.Parse(results[feet]);
            int inchesResult = int.Parse(results[inches]);

            Assert.AreEqual(5, feetResult);
            Assert.AreEqual(6, inchesResult);
        }
    }
}