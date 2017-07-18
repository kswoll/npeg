using System.Collections;
using System.Collections.Generic;
using PEG.SyntaxTree;

namespace PEG
{
    public class ExpressionEnumerator : ExpressionWalker<object>, IEnumerable<Expression>
    {
        private readonly List<Expression> expressions = new List<Expression>();

        public ExpressionEnumerator(Expression root)
        {
            root.Accept(this, null);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Expression> GetEnumerator()
        {
            return expressions.GetEnumerator();
        }

        public override void Visit(AndPredicate expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(AnyCharacter expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(CharacterSet expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(CharacterTerminal expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(EmptyString expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(Nonterminal expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(NotPredicate expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(OneOrMore expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(Optional expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(OrderedChoice expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(Sequence expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(Terminal expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(ZeroOrMore expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }

        public override void Visit(Repeat expression, object context)
        {
            expressions.Add(expression);
            base.Visit(expression, context);
        }
    }
}