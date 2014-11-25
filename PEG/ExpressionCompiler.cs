using PEG.SyntaxTree;

namespace PEG
{
    public class ExpressionCompiler : ExpressionWalker<object>
    {
        private int nonterminalIndex;

        public override void Visit(Nonterminal expression, object context)
        {
            expression.Index = nonterminalIndex++;
            base.Visit(expression, context);
        }
    }
}