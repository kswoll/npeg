using NUnit.Framework;

namespace PEG.Tests
{
    [TestFixture]
    public class ArgumentsTest
    {
        public class ArgumentsGrammar : Grammar<ArgumentsGrammar>
        {
            public string Argument { get; set; }

            protected override void OnCreated(object[] args)
            {
                base.OnCreated(args);

                Argument = (string)args[0];
            }
        }

        [Test]
        public void Arguments()
        {
            var grammar = ArgumentsGrammar.Create("foo");
            Assert.AreEqual("foo", grammar.Argument);
        }
    }
}