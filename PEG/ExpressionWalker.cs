using PEG.SyntaxTree;

namespace PEG
{
    public class ExpressionWalker<T> : IExpressionVisitor<T>
    {
        public virtual void Visit(AndPredicate expression, T context)
        {
            expression.Operand.Accept(this, context);
        }

        public virtual void Visit(AnyCharacter expression, T context)
        {
        }

        public virtual void Visit(CharacterSet expression, T context)
        {
            foreach (var c in expression.Characters)
                c.Accept(this, context);
        }

        public virtual void Visit(CharacterTerminal expression, T context)
        {
        }

        public virtual void Visit(EmptyString expression, T context)
        {
        }

        public virtual void Visit(Nonterminal expression, T context)
        {
            expression.Expression.Accept(this, context);
        }

        public virtual void Visit(NotPredicate expression, T context)
        {
            expression.Operand.Accept(this, context);
        }

        public virtual void Visit(OneOrMore expression, T context)
        {
            expression.Operand.Accept(this, context);
        }

        public virtual void Visit(Optional expression, T context)
        {
            expression.Operand.Accept(this, context);
        }

        public virtual void Visit(OrderedChoice expression, T context)
        {
            foreach (var current in expression.Expressions)
                current.Accept(this, context);
        }

        public virtual void Visit(Sequence expression, T context)
        {
            foreach (var current in expression.Expressions)
                current.Accept(this, context);
        }

        public virtual void Visit(Terminal expression, T context)
        {
        }

        public virtual void Visit(ZeroOrMore expression, T context)
        {
            expression.Operand.Accept(this, context);
        }

        public virtual void Visit(Repeat expression, T context)
        {
            expression.Operand.Accept(this, context);
        }
    }
}