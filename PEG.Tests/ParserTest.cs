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
            Assert.IsNotNull(parser.ParseString("a"));
            Assert.IsNull(parser.ParseString("b"));
        }

        [Test]
        public void LetterChoice()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.LetterChoice()));
            Assert.IsNotNull(parser.ParseString("a"));
            Assert.IsNotNull(parser.ParseString("b"));
            Assert.IsNull(parser.ParseString("c"));
        }

        [Test]
        public void NonterminalAndLetterChoice()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.NonterminalAndLetterChoice()));
            Assert.IsNotNull(parser.ParseString("a"));
            Assert.IsNotNull(parser.ParseString("b"));
            Assert.IsNull(parser.ParseString("c"));
        }

        [Test]
        public void NonterminalAndNonterminalChoice()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.NonterminalAndNonterminalChoice()));
            Assert.IsNotNull(parser.ParseString("a"));
            Assert.IsNotNull(parser.ParseString("b"));
            Assert.IsNull(parser.ParseString("c"));
        }

        [Test]
        public void LetterSequence()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.LetterSequence()));
            Assert.IsNotNull(parser.ParseString("ab"));
            Assert.IsNull(parser.ParseString("a"));
            Assert.IsNull(parser.ParseString("b"));
        }

        [Test]
        public void NotLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.NotLetterA()));
            Assert.IsNull(parser.ParseString("a"));
            Assert.IsNotNull(parser.ParseString("b", false));
        }

        [Test]
        public void AndLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.AndLetterA()));
            Assert.IsNotNull(parser.ParseString("a", false));
            Assert.IsNull(parser.ParseString("b"));
        }

        [Test]
        public void OneOrMoreLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.OneOrMoreLetterA()));
            Assert.IsNotNull(parser.ParseString("a"));
            Assert.IsNotNull(parser.ParseString("aa"));
            Assert.IsNotNull(parser.ParseString("aaa"));
            Assert.IsNull(parser.ParseString("b"));
            Assert.IsNull(parser.ParseString(""));
        }

        [Test]
        public void ZeroOrMoreLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.ZeroOrMoreLetterA()));
            Assert.IsNotNull(parser.ParseString(""));
            Assert.IsNotNull(parser.ParseString("a"));
            Assert.IsNotNull(parser.ParseString("aa"));
            Assert.IsNotNull(parser.ParseString("aaa"));
        }

        [Test]
        public void LeftRecursion()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            PegParser parser = new PegParser(grammar, grammar.GetNonterminal(o => o.LeftRecursion()));
            Assert.IsNotNull(parser.ParseString("a"));
            Assert.IsNotNull(parser.ParseString("aa"));
            Assert.IsNotNull(parser.ParseString("aaa"));
        }
    }
}