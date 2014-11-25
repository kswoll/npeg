using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PEG.SyntaxTree
{
    /// <summary>
    /// Base class for all PEG expressions.  
    /// 
    /// The operator overloading contained here attempts to approximate the actual PEG grammar within 
    /// the syntax of C#.  The normal PEG operators have been changed as follows:
    /// 
    /// sequence: No operator, just expressions followed in succession.  
    ///     This would be illegal in C# so the '+' operator is used to separate expressions in a sequence.
    /// ordered choice: '/'
    ///     The precedence of this operator in C# would be far too high so has been changed to the traditional 
    ///     pipe character ('|') which has very low precedence.
    /// one or more: '+'
    ///     In C# it's the same, but the unary operator must precede the expression, rather than be a suffix.
    ///     i.e.  +a instead of a+.
    /// zero or more: '*'
    ///     In C#, '*' is not a valid unary operator, so has been changed to '-'.  Use as a prefix.
    /// optional: '?'
    ///     In C#, '?' is not a valid unary operator, so has been changed to '~'.  Again, as a prefix.
    /// and predicate: '&'
    ///     We have run out of operators, so we will use a extension method named "And".
    /// not predicate: '!'
    ///     Exactly the same.
    ///     
    /// </summary>
    public abstract class Expression 
    {
        public abstract IEnumerable<OutputRecord> Execute(ParseEngine engine);
        public abstract void Accept<T>(IExpressionVisitor<T> visitor, T context);

        protected static IEnumerable<OutputRecord> NoResults = Enumerable.Empty<OutputRecord>();

        public static implicit operator Expression(char c)
        {
            return new CharacterTerminal(c);
        }

        public static implicit operator Expression(string seq)
        {
            return (Sequence)seq;
        }

        public static Expression operator +(Expression left, char right)
        {
            return left + (Terminal)right;
        }

        public static Expression operator +(char left, Expression right)
        {
            return (Terminal)left + right;
        }

        public static Expression operator +(Expression left, Expression right)
        {
            // Coalesce nested sequences
            if (left is Sequence)
            {
                Sequence existing = (Sequence)left;
                existing.Expressions.Add(right);
                return existing;
            }
            else
            {
                Sequence sequence = new Sequence();
                sequence.Expressions.Add(left);
                sequence.Expressions.Add(right);
                return sequence;                
            }
        }

        public static Expression operator |(Expression left, char right)
        {
            return left | (Terminal)right;
        }

        public static Expression operator |(char left, Expression right)
        {
            return (Terminal)left | right;
        }

        public static Expression operator |(Expression left, string right)
        {
            return left | (Sequence)right;
        }

        public static Expression operator |(string left, Expression right)
        {
            return (Sequence)left | right;
        }

        public static Expression operator |(Expression left, Expression right)
        {
            // Coalesce nested ordered choices
            if (left is OrderedChoice)
            {
                OrderedChoice existing = (OrderedChoice)left;
                existing.Expressions.Add(right);
                return existing;
            }
            else
            {
                OrderedChoice orderedChoice = new OrderedChoice();
                orderedChoice.Expressions.Add(left);
                orderedChoice.Expressions.Add(right);
                return orderedChoice;                
            }
        }

        /// <summary>
        /// Applies the <i>one-or-more</i> operator onto the operand (the expression that 
        /// follows this operator).
        /// </summary>
        public static Expression operator +(Expression operand)
        {
            OneOrMore oneOrMore = new OneOrMore();
            oneOrMore.Operand = operand;
            return oneOrMore;
        }

        public static Expression operator -(Expression operand)
        {
            ZeroOrMore zeroOrMore = new ZeroOrMore();
            zeroOrMore.Operand = operand;
            return zeroOrMore;
        }

        public static Expression operator !(Expression operand)
        {
            NotPredicate notPredicate = new NotPredicate();
            notPredicate.Operand = operand;
            return notPredicate;
        }

        public static Expression operator ~(Expression operand)
        {
            Optional optional = new Optional();
            optional.Operand = operand;
            return optional;
        }
    }
}