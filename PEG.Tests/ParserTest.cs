using NUnit.Framework;
using PEG.SyntaxTree;

namespace PEG.Tests
{
    [TestFixture]
    public class ParserTest
    {
        public class TestGrammar1 : Grammar<TestGrammar1>
        {
            public virtual Expression LetterA()
            {
                return 'a';
            }

            public virtual Expression LetterB()
            {
                return 'b';
            }

            public virtual Expression LetterChoice()
            {
                return 'a'._() | 'b';
            }

            public virtual Expression NonterminalAndLetterChoice()
            {
                return LetterA() | 'b';
            }

            public virtual Expression NonterminalAndNonterminalChoice()
            {
                return LetterA() | LetterB();
            }

            public virtual Expression LetterSequence()
            {
                return 'a'._() + 'b';
            }

            public virtual Expression NonterminalAndLetterSequence()
            {
                return LetterA() + 'b';
            }

            public virtual Expression NonterminalAndNonterminalSequence()
            {
                return LetterA() + LetterB();
            }

            public virtual Expression NotLetterA()
            {
                return !LetterA();
            }

            public virtual Expression AndLetterA()
            {
                return LetterA().And();
            }

            public virtual Expression OneOrMoreLetterA()
            {
                return +LetterA();
            }

            public virtual Expression ZeroOrMoreLetterA()
            {
                return -LetterA();
            }

            public virtual Expression OptionalLetterA()
            {
                return ~LetterA();
            }

            public virtual Expression TwoSequences()
            {
                return LetterA() + LetterB() | LetterB() + LetterA();
            }

            public virtual Expression ThreeChoices()
            {
                return LetterA() | LetterB() | 'c';
            }

            public virtual Expression ThreeExpressionSequence()
            {
                return LetterA() + LetterB() + 'c';
            }

            public virtual Expression LeftRecursion()
            {
                return LeftRecursion() + LetterA() | LetterA();
            }
        }

        [Test]
        public void LetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.LetterA()));
            Assert.IsFalse(parser.ParseString("a").span.IsFailed);
            Assert.IsTrue(parser.ParseString("b").span.IsFailed);
        }

        [Test]
        public void LetterChoice()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.LetterChoice()));
            Assert.IsFalse(parser.ParseString("a").span.IsFailed);
            Assert.IsFalse(parser.ParseString("b").span.IsFailed);
            Assert.IsTrue(parser.ParseString("c").span.IsFailed);
        }

        [Test]
        public void NonterminalAndLetterChoice()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.NonterminalAndLetterChoice()));
            Assert.IsFalse(parser.ParseString("a").span.IsFailed);
            Assert.IsFalse(parser.ParseString("b").span.IsFailed);
            Assert.IsTrue(parser.ParseString("c").span.IsFailed);
        }

        [Test]
        public void NonterminalAndNonterminalChoice()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.NonterminalAndNonterminalChoice()));
            Assert.IsFalse(parser.ParseString("a").span.IsFailed);
            Assert.IsFalse(parser.ParseString("b").span.IsFailed);
            Assert.IsTrue(parser.ParseString("c").span.IsFailed);
        }

        [Test]
        public void LetterSequence()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.LetterSequence()));
            Assert.IsFalse(parser.ParseString("ab").span.IsFailed);
            Assert.IsTrue(parser.ParseString("a").span.IsFailed);
            Assert.IsTrue(parser.ParseString("b").span.IsFailed);
        }

        [Test]
        public void NotLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.NotLetterA()));
            Assert.IsTrue(parser.ParseString("a").span.IsFailed);
            Assert.IsFalse(parser.ParseString("b", false).span.IsFailed);
        }

        [Test]
        public void AndLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.AndLetterA()));
            Assert.IsFalse(parser.ParseString("a", false).span.IsFailed);
            Assert.IsTrue(parser.ParseString("b").span.IsFailed);
        }

        [Test]
        public void OneOrMoreLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.OneOrMoreLetterA()));
            Assert.IsFalse(parser.ParseString("a").span.IsFailed);
            Assert.IsFalse(parser.ParseString("aa").span.IsFailed);
            Assert.IsFalse(parser.ParseString("aaa").span.IsFailed);
            Assert.IsTrue(parser.ParseString("b").span.IsFailed);
            Assert.IsTrue(parser.ParseString("").span.IsFailed);
        }

        [Test]
        public void ZeroOrMoreLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.ZeroOrMoreLetterA()));
            Assert.IsFalse(parser.ParseString("").span.IsFailed);
            Assert.IsFalse(parser.ParseString("a").span.IsFailed);
            Assert.IsFalse(parser.ParseString("aa").span.IsFailed);
            Assert.IsFalse(parser.ParseString("aaa").span.IsFailed);
        }

        [Test]
        public void LeftRecursion()
        {
            var grammar = TestGrammar1.Create();
            var parser = new PegParser(grammar, grammar.GetNonterminal(o => o.LeftRecursion()));
            Assert.IsFalse(parser.ParseString("a").span.IsFailed);
            Assert.IsFalse(parser.ParseString("aa").span.IsFailed);
            Assert.IsFalse(parser.ParseString("aaa").span.IsFailed);
        }
    }
}