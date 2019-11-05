using NUnit.Framework;

namespace PEG.Tests.CppMacro
{
    [TestFixture]
    public class MacroGrammarTest
    {
        [Test]
        public void AndBuiltInIdentifier()
        {
            var macro = Macro.Parse("ASMJIT_CXX_CLANG && defined(__has_builtin)");
            Assert.AreEqual("(ASMJIT_CXX_CLANG && defined(__has_builtin))", macro.ToString());
        }

        [Test]
        public void AndNot()
        {
            var macro = Macro.Parse("defined(ASMJIT_NO_TEXT) && !defined(ASMJIT_NO_LOGGING)");
            Assert.AreEqual("(defined(ASMJIT_NO_TEXT) && !(defined(ASMJIT_NO_LOGGING)))", macro.ToString());
        }

        [Test]
        public void AndNotOr()
        {
            var macro = Macro.Parse(
                "(defined(_M_IX86) || defined(_M_AMD64) || defined(_M_ARM)) && !defined(MIDL_PASS)"
            );
            Assert.AreEqual(
                "((defined(_M_IX86) || defined(_M_AMD64) || defined(_M_ARM)) && !(defined(MIDL_PASS)))",
                macro.ToString()
            );
        }

        [Test]
        public void BuiltInIdentifier()
        {
            var macro = Macro.Parse("__cplusplus");
            Assert.AreEqual("__cplusplus", macro.ToString());
        }

        [Test]
        public void Defined()
        {
            var macro = Macro.Parse("defined(_WIN32)");
            Assert.AreEqual("defined(_WIN32)", macro.ToString());
        }

        [Test]
        public void DefinedSpace()
        {
            var macro = Macro.Parse("defined (_WIN64)");
            Assert.AreEqual("defined(_WIN64)", macro.ToString());
        }
    }
}