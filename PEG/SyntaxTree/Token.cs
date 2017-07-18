using System.Collections.Generic;
using PEG.Cst;

namespace PEG.SyntaxTree
{
    public class Token : Terminal, ICstNonterminalNode
    {
        public int AbsoluteIndex { get; set; }
        public Nonterminal TokenRule { get; set; }
        public IEnumerable<OutputRecord> Lexeme { get; set; }
        public int Position { get; set; }

        private CstNonterminalNode cstNode;

        public Token(Nonterminal tokenRule, IEnumerable<OutputRecord> lexeme, int absoluteIndex)
        {
            TokenRule = tokenRule;
            Lexeme = lexeme;
            AbsoluteIndex = absoluteIndex;
        }

        public Nonterminal Nonterminal => TokenRule;

        public List<ICstNode> Children => Cst.Children;

        public override ParseOutputSpan Execute(ParseEngine engine)
        {
            if (engine.Current is Token && ((Token)engine.Current).TokenRule == TokenRule)
            {
                var position = engine.Position;
                if (engine.Consume())
                    return OutputResult(engine.Output, position);
                else
                    return engine.False;
            }
            return engine.False;
        }

        public override string Coalesce()
        {
            return Cst.Coalesce();
        }

        public CstNonterminalNode Cst
        {
            get
            {
                if (cstNode == null)
                {
                    cstNode = CstBuilder.Build(Lexeme);
                }
                return cstNode;
            }
        }

        public override string ToString()
        {
            return Coalesce();
        }

        public override void Accept<T>(IExpressionVisitor<T> visitor, T context)
        {
            visitor.Visit(this, context);
        }
    }
}