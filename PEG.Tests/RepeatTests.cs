using System.Collections.Generic;
using NUnit.Framework;
using PEG.SyntaxTree;

namespace PEG.Tests
{
    [TestFixture]
    public class RepeatTests
    {
        [Test]
        public void RepeatResets()
        {
            var grammar = TestGrammar.Create();
            var parser = new PegParser<TestData>(grammar, grammar.Root());
            var input = "AAABBBBB";
            var result = parser.Parse(input);
            Assert.AreEqual(input, result.Items);
        }

        public class TestData
        {
            public string Items { get; set; }
        }

        public class TestGrammar : Grammar<TestGrammar>
        {
            public virtual Expression Root()
            {
                return Items();
            }

            public virtual Expression Items()
            {
                return -(A() | B());
            }

            public virtual Expression A()
            {
                return 'A'._().Repeat(4, 6);
            }

            public virtual Expression B()
            {
                return 'A'._() | 'B';
            }
        }
    }
}