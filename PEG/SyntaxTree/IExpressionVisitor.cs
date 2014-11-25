namespace PEG.SyntaxTree
{
    public interface IExpressionVisitor<T>
    {
        void Visit(AndPredicate expression, T context);
        void Visit(AnyCharacter expression, T context);
        void Visit(CharacterSet expression, T context);
        void Visit(CharacterTerminal expression, T context);
        void Visit(EmptyString expression, T context);
        void Visit(Nonterminal expression, T context);
        void Visit(NotPredicate expression, T context);
        void Visit(OneOrMore expression, T context);
        void Visit(Optional expression, T context);
        void Visit(OrderedChoice expression, T context);
        void Visit(Sequence expression, T context);
        void Visit(Terminal expression, T context);
        void Visit(Token expression, T context);
        void Visit(ZeroOrMore expression, T context);
        void Visit(ForeignNonterminal expression, T context);
        void Visit(Substitution expression, T context);
        void Visit(Repeat expression, T context);
        void Visit(EncloseExpression expression, T context);
    }
}