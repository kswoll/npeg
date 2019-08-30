using NUnit.Framework;
using PEG.SyntaxTree;

namespace PEG.Tests
{
    [TestFixture]
    public class CaptureTests
    {
        [Test]
        public void CaptureInitializedProperly()
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

/*
            public virtual Expression A()
            {
                return 'A'._().Repeat(4, 6);
            }

            public virtual Expression B()
            {
                return 'A'._() | 'B';
            }
*/

            public virtual Expression Items()
            {
                return -('A'._().Repeat(4, 6).Capture("A") | ('A'._() | 'B').Capture("B"));
            }
        }
    }
}