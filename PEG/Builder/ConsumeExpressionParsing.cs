using System;
using PEG.Cst;
using PEG.SyntaxTree;

namespace PEG.Builder
{
    public class ConsumeExpressionParsing : Grammar<ConsumeExpressionParsing>
    {
        public virtual Expression Choice()
        {
            return Choice() + ~WS() + '|' + ~WS() + Conditional() | Conditional();
        }

        public virtual Expression Conditional()
        {
            return Expression() | Expression() + ~WS() + ':' + ~WS() + Expression();
        }

        public virtual Expression Expression()
        {
            return IndexExpression() | ReferenceExpression();
        }

        public virtual Expression IndexExpression()
        {
            return Expression() + ~WS() + '[' + ~WS() + Number() + ~WS() + ']';
        }

        public virtual Expression ReferenceExpression()
        {
            return Expression() + '.' + Identifier() | Identifier();
        }

        public virtual Expression Identifier()
        {
            return IdentifierStartChar() + -IdentifierChar();
        }

        public virtual Expression IdentifierChar()
        {
            return IdentifierStartChar() | Digit() | '-';
        }

        public virtual Expression IdentifierStartChar()
        {
            return 'a'.To('z') | 'A'.To('Z');
        }

        public virtual Expression Digit()
        {
            return '0'.To('9');
        }

        public virtual Expression Number()
        {
            return +Digit();
        }

        public virtual Expression WS()
        {
            return +WhitespaceChar();
        }

        public virtual Expression WhitespaceChar()
        {
            return ' ';
        }

        public static ConsumeExpressionParsing Grammar = Create();
        public static PegParser Parser = new PegParser(Grammar, Grammar.Choice());
        //        public static PegBuilder<ConsumeChoice> Builder = new PegBuilder<ConsumeChoice>(Grammar);

        public static ConsumeExpression Parse(string s)
        {
            var result = Parser.ParseString(s);

            // Bypass BnfBuilder because that depends on this in order to work
            CstNonterminalNode cst = CstBuilder.Build(result);
            return BuildChoice(cst);
        }

        private static ConsumeChoice BuildChoice(CstNonterminalNode node)
        {
            ConsumeChoice result = new ConsumeChoice();
            CstNonterminalNode current = node;
            while (current != null)
            {
                if (current.Children.Count == 1)
                {
                    result.Choices.Insert(0, BuildConditional((CstNonterminalNode)current.Children[0]));
                    current = null;
                }
                else
                {
                    result.Choices.Insert(0, BuildConditional((CstNonterminalNode)current.Children[2]));
                    current = (CstNonterminalNode)current.Children[0];
                }
            }
            return result;
        }

        private static ConsumeConditional BuildConditional(CstNonterminalNode node)
        {
            CstNonterminalNode predicate;
            CstNonterminalNode expression;
            if (node.Children.Count == 1)
            {
                predicate = null;
                expression = (CstNonterminalNode)node.Children[0];
            }
            else
            {
                predicate = (CstNonterminalNode)node.Children[0];
                expression = (CstNonterminalNode)node.Children[2];
            }
            ConsumeConditional conditional = new ConsumeConditional();
            if (predicate != null)
                conditional.Predicate = BuildExpression(predicate);
            conditional.Expression = BuildExpression(expression);
            return conditional;
        }

        private static ConsumeExpression BuildExpression(CstNonterminalNode node)
        {
            CstNonterminalNode expressionType = (CstNonterminalNode)node.Children[0];
            if (expressionType.Nonterminal.Name == "Expression")
                expressionType = (CstNonterminalNode)expressionType.Children[0];
            switch (expressionType.Nonterminal.Name)
            {
                case "ReferenceExpression":
                    return BuildReferenceExpression(expressionType);
                case "IndexExpression":
                    return BuildIndexExpression(expressionType);
                default:
                    throw new InvalidOperationException();
            }
        }

        private static ConsumeReferenceExpression BuildReferenceExpression(CstNonterminalNode node)
        {
            ConsumeReferenceExpression expression = new ConsumeReferenceExpression();
            if (node.Children.Count == 1)
            {
                //                CstNonterminalNode nonterminal = (CstNonterminalNode)((Token)((ICstTerminalNode)node.Children[0]).Terminal).StructuredLexeme;
                expression.NonTerminal = node.Children[0].Coalesce();
            }
            else
            {
                CstNonterminalNode targetNode = (CstNonterminalNode)node.Children[0];
                ConsumeExpression target = BuildExpression(targetNode);
                expression.Target = target;

                //                CstNonterminalNode nonterminal = (CstNonterminalNode)((Token)((ICstTerminalNode)node.Children[2]).Terminal).StructuredLexeme;
                expression.NonTerminal = node.Children[2].Coalesce();//nonterminal.Coalesce();
            }
            return expression;
        }

        private static ConsumeIndexExpression BuildIndexExpression(CstNonterminalNode node)
        {
            ConsumeIndexExpression expression = new ConsumeIndexExpression();

            CstNonterminalNode targetNode = (CstNonterminalNode)node.Children[0];
            expression.Target = BuildExpression(targetNode);

            //            CstNonterminalNode indexNode = (CstNonterminalNode)((Token)((ICstTerminalNode)node.Children[2]).Terminal).StructuredLexeme;
            expression.Index = int.Parse(node.Children[2].Coalesce());

            return expression;
        }
    }
}