using System.Diagnostics.CodeAnalysis;
using PEG.SyntaxTree;

namespace PEG.Tests.CppMacro
{
    [SuppressMessage("ReSharper", "FunctionRecursiveOnAllPaths")]
    public class MacroGrammar : Grammar<MacroGrammar>
    {
        /// <summary>
        /// You should always make the constructor private so that you never accidentally instantiate it directly.  
        /// Always use the .Create() method inherited from Grammar.  i.e.  MacroGrammar.Create();
        /// </summary>
        private MacroGrammar()
        {
        }

        public virtual Expression Root()
        {
            return Or();
        }

        public virtual Expression And()
        {
            return PrimitiveCondition() + -("&&" + PrimitiveCondition());
        }

        public virtual Expression Or()
        {
            return And() + -("||" + And());
        }

        public virtual Expression PrimitiveCondition()
        {
            return ~Space() + (('(' + Or() + ')') | Defined() | NegateCondition() | Identifier()) + ~Space();
        }

        public virtual Expression NegateCondition()
        {
            return '!' + Or();
        }

        public virtual Expression Defined()
        {
            return ~Space() + "defined" + IdentifierWrap();
        }

        public virtual Expression IdentifierWrap()
        {
            return ~Space() + (('(' + IdentifierWrap() + ')') | Identifier()) + ~Space();
        }

        public virtual Expression Identifier()
        {
            return IdentifierStartChar() + -IdentifierChar();
        }

        public virtual Expression IdentifierChar()
        {
            return IdentifierStartChar() | '0'.To('9');
        }

        public virtual Expression IdentifierStartChar()
        {
            return 'a'.To('Z') | '_';
        }

        public virtual Expression Space()
        {
            return +(' '._() | '\r' | '\n' | '\t');
        }
    }
}