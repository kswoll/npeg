using NUnit.Framework;
using PEG.SyntaxTree;

namespace PEG.Tests
{
    [TestFixture]
    public class CodeGrammarTest
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
        }

        [Test]
        public void TestGrammar1Letter()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.LetterA());
            Assert.AreEqual("LetterA", nonterminal.Name);
            Assert.AreEqual('a', ((CharacterTerminal)nonterminal.Expression).Character);
        }

        [Test]
        public void TestGrammar1LetterChoice()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.LetterChoice());
            Assert.AreEqual("LetterChoice", nonterminal.Name);
            OrderedChoice orderedChoice = (OrderedChoice)nonterminal.Expression;
            Assert.AreEqual('a', ((CharacterTerminal)orderedChoice.Expressions[0]).Character);
            Assert.AreEqual('b', ((CharacterTerminal)orderedChoice.Expressions[1]).Character);
        }

        [Test]
        public void TestGrammar1NonterminalAndLetterChoice()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.NonterminalAndLetterChoice());
            Assert.AreEqual("NonterminalAndLetterChoice", nonterminal.Name);
            OrderedChoice orderedChoice = (OrderedChoice)nonterminal.Expression;
            Assert.AreEqual("LetterA", ((Nonterminal)orderedChoice.Expressions[0]).Name);
            Assert.AreEqual('b', ((CharacterTerminal)orderedChoice.Expressions[1]).Character);
        }

        [Test]
        public void TestGrammar1NonterminalAndNonterminalChoice()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.NonterminalAndNonterminalChoice());
            Assert.AreEqual("NonterminalAndNonterminalChoice", nonterminal.Name);
            OrderedChoice orderedChoice = (OrderedChoice)nonterminal.Expression;
            Assert.AreEqual("LetterA", ((Nonterminal)orderedChoice.Expressions[0]).Name);
            Assert.AreEqual("LetterB", ((Nonterminal)orderedChoice.Expressions[1]).Name);
        }

        [Test]
        public void TestGrammar1LetterSequence()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.LetterSequence());
            Assert.AreEqual("LetterSequence", nonterminal.Name);
            Sequence orderedChoice = (Sequence)nonterminal.Expression;
            Assert.AreEqual('a', ((CharacterTerminal)orderedChoice.Expressions[0]).Character);
            Assert.AreEqual('b', ((CharacterTerminal)orderedChoice.Expressions[1]).Character);
        }

        [Test]
        public void TestGrammar1NonterminalAndLetterSequence()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.NonterminalAndLetterSequence());
            Assert.AreEqual("NonterminalAndLetterSequence", nonterminal.Name);
            Sequence orderedChoice = (Sequence)nonterminal.Expression;
            Assert.AreEqual("LetterA", ((Nonterminal)orderedChoice.Expressions[0]).Name);
            Assert.AreEqual('b', ((CharacterTerminal)orderedChoice.Expressions[1]).Character);
        }

        [Test]
        public void TestGrammar1NonterminalAndNonterminalSequence()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.NonterminalAndNonterminalSequence());
            Assert.AreEqual("NonterminalAndNonterminalSequence", nonterminal.Name);
            Sequence orderedChoice = (Sequence)nonterminal.Expression;
            Assert.AreEqual("LetterA", ((Nonterminal)orderedChoice.Expressions[0]).Name);
            Assert.AreEqual("LetterB", ((Nonterminal)orderedChoice.Expressions[1]).Name);
        }

        [Test]
        public void TestGrammar1NotLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.NotLetterA());
            Assert.AreEqual("NotLetterA", nonterminal.Name);
            NotPredicate expression = (NotPredicate)nonterminal.Expression;
            Assert.AreEqual("LetterA", ((Nonterminal)expression.Operand).Name);
        }

        [Test]
        public void TestGrammarAndLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.AndLetterA());
            Assert.AreEqual("AndLetterA", nonterminal.Name);
            AndPredicate expression = (AndPredicate)nonterminal.Expression;
            Assert.AreEqual("LetterA", ((Nonterminal)expression.Operand).Name);
        }

        [Test]
        public void TestGrammarOneOrMoreLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.OneOrMoreLetterA());
            Assert.AreEqual("OneOrMoreLetterA", nonterminal.Name);
            OneOrMore expression = (OneOrMore)nonterminal.Expression;
            Assert.AreEqual("LetterA", ((Nonterminal)expression.Operand).Name);
        }

        [Test]
        public void TestGrammarZeroOrMoreLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.ZeroOrMoreLetterA());
            Assert.AreEqual("ZeroOrMoreLetterA", nonterminal.Name);
            ZeroOrMore expression = (ZeroOrMore)nonterminal.Expression;
            Assert.AreEqual("LetterA", ((Nonterminal)expression.Operand).Name);
        }

        [Test]
        public void TestGrammarOptionalLetterA()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.OptionalLetterA());
            Assert.AreEqual("OptionalLetterA", nonterminal.Name);
            Optional expression = (Optional)nonterminal.Expression;
            Assert.AreEqual("LetterA", ((Nonterminal)expression.Operand).Name);
        }

        [Test]
        public void TestGrammarTwoSequences()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.TwoSequences());
            Assert.AreEqual("TwoSequences", nonterminal.Name);
            OrderedChoice orderedChoice = (OrderedChoice)nonterminal.Expression;
            Sequence sequence1 = (Sequence)orderedChoice.Expressions[0];
            Sequence sequence2 = (Sequence)orderedChoice.Expressions[1];
            Assert.AreEqual("LetterA", ((Nonterminal)sequence1.Expressions[0]).Name);
            Assert.AreEqual("LetterB", ((Nonterminal)sequence1.Expressions[1]).Name);
            Assert.AreEqual("LetterB", ((Nonterminal)sequence2.Expressions[0]).Name);
            Assert.AreEqual("LetterA", ((Nonterminal)sequence2.Expressions[1]).Name);
        }

        [Test]
        public void TestGrammarThreeChoices()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.ThreeChoices());
            Assert.AreEqual("ThreeChoices", nonterminal.Name);
            OrderedChoice orderedChoice = (OrderedChoice)nonterminal.Expression;
            Assert.AreEqual("LetterA", ((Nonterminal)orderedChoice.Expressions[0]).Name);
            Assert.AreEqual("LetterB", ((Nonterminal)orderedChoice.Expressions[1]).Name);
            Assert.AreEqual('c', ((CharacterTerminal)orderedChoice.Expressions[2]).Character);
        }

        [Test]
        public void TestGrammarThreeExpressionSequence()
        {
            TestGrammar1 grammar = TestGrammar1.Create();
            Nonterminal nonterminal = grammar.GetNonterminal(o => o.ThreeExpressionSequence());
            Assert.AreEqual("ThreeExpressionSequence", nonterminal.Name);
            Sequence sequence = (Sequence)nonterminal.Expression;
            Assert.AreEqual("LetterA", ((Nonterminal)sequence.Expressions[0]).Name);
            Assert.AreEqual("LetterB", ((Nonterminal)sequence.Expressions[1]).Name);
            Assert.AreEqual('c', ((CharacterTerminal)sequence.Expressions[2]).Character);
        }
    }
}