using PEG.SyntaxTree;

namespace PEG.Samples.Url
{
    /// <summary>
    /// Provides the grammar for a URL
    /// </summary>
    public class UrlGrammar : Grammar<UrlGrammar>
    {
        /// <summary>
        /// You should always make the constructor private so that you never accidentally instantiate it directly.  
        /// Always use the .Create() method inherited from Grammar.  i.e.  UrlGrammar.Create();
        /// </summary>
        private UrlGrammar()
        {
        }

        public virtual Expression Url()
        {
            return Protocol() + "://" + Domain() + ~(':' + Port()) + ~(Path() + ~('?' + QueryString()));
        }

        public virtual Expression Protocol()
        {
            return "http"._() | "https";
        }

        public virtual Expression Domain()
        {
            return Domain() + '.' + DomainWord() | DomainWord();
        }

        public virtual Expression DomainWord()
        {
            return +('a'.To('z') | 'A'.To('Z') | '0'.To('9') | '-');
        }

        public virtual Expression Port()
        {
            return +('0'.To('9'));
        }

        public virtual Expression Path()
        {
            return +('/' + +('a'.To('z') | 'A'.To('Z') | '0'.To('9') | '.' | '_'));
        }

        public virtual Expression QueryString()
        {
            return NameValue() + -('&' + NameValue());
        }

        public virtual Expression NameValue()
        {
            return Name() + '=' + Value();
        }

        public virtual Expression Name()
        {
            return +('a'.To('z') | 'A'.To('Z') | '0'.To('9'));
        }

        public virtual Expression Value()
        {
            return -('a'.To('z') | 'A'.To('Z') | '0'.To('9') | '%');
        }
    }
}