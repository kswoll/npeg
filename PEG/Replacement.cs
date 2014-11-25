using PEG.SyntaxTree;

namespace PEG
{
    public class Replacement
    {
        public Nonterminal From { get; set; }
        public IReplacementTarget To { get; set; }
    }
}