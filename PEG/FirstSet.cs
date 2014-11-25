using System;
using PEG.SyntaxTree;
using PEG.Utils;

namespace PEG
{
    /// <summary>
    /// NOT FUNCTIONAL YET
    /// Future plan to implement first sets in the PEG parser for choice operation optimizations.  
    /// </summary>
    public class FirstSet : IExpressionVisitor<Nonterminal>
    {
        public DictionarySet<Nonterminal, Terminal> nonterminalSet = new DictionarySet<Nonterminal, Terminal>();
        public DictionarySet<Expression, Terminal> expressionSet = new DictionarySet<Expression, Terminal>();
        
        private int totalCount;

        public FirstSet(Grammar grammar)
        {
            int lastCount;
            do
            {
                lastCount = totalCount;
                foreach (Nonterminal nonterminal in grammar.Nonterminals)
                {
                    nonterminal.Expression.Accept(this, nonterminal);
                }
            }
            while (lastCount != totalCount);
        }

        private void Add(Nonterminal key, Expression expression, Terminal value)
        {
            nonterminalSet[key].Add(value);
            if (expressionSet[expression].Add(value))
                totalCount++;                
        }

        public bool Contains(Nonterminal expression, Terminal terminal)
        {
            return nonterminalSet[expression].Contains(terminal);
        }

        public void Visit(AndPredicate expression, Nonterminal context)
        {
            expression.Operand.Accept(this, context);
        }

        public void Visit(AnyCharacter expression, Nonterminal context)
        {
            Add(context, expression, AnyCharacterTerminal.Instance);
        }

        public void Visit(CharacterSet expression, Nonterminal context)
        {
            foreach (var character in expression.Characters)
                Add(context, expression, character);
        }

        public void Visit(CharacterTerminal expression, Nonterminal context)
        {
            Add(context, expression, expression);
        }

        public void Visit(EmptyString expression, Nonterminal context)
        {
            Add(context, expression, NullTerminal.Instance);
        }

        public void Visit(Nonterminal expression, Nonterminal context)
        {
            // Intentionally do not want to traverse nonterminals since that is handled in the primary loop 
            // (prevent infinite recursion)
        }

        public void Visit(NotPredicate expression, Nonterminal context)
        {
            expression.Operand.Accept(this, context);
        }

        public void Visit(OneOrMore expression, Nonterminal context)
        {
            expression.Operand.Accept(this, context);
        }

        public void Visit(Optional expression, Nonterminal context)
        {
            Add(context, expression, NullTerminal.Instance);
            expression.Operand.Accept(this, context);
        }

        public void Visit(OrderedChoice expression, Nonterminal context)
        {
            foreach (var item in expression.Expressions)
                item.Accept(this, context);
        }

        public void Visit(Sequence expression, Nonterminal context)
        {
            foreach (var item in expression.Expressions)
            {
                item.Accept(this, context);
                if (!expressionSet[item].Contains(NullTerminal.Instance))
                    break;
            }
        }

        public void Visit(Terminal expression, Nonterminal context)
        {
            Add(context, expression, expression);
        }

        public void Visit(Token expression, Nonterminal context)
        {
            Add(context, expression, expression);
        }

        public void Visit(ZeroOrMore expression, Nonterminal context)
        {
            expression.Operand.Accept(this, context);
        }

        public void Visit(ForeignNonterminal expression, Nonterminal context)
        {
        }

        public void Visit(Substitution expression, Nonterminal context)
        {
        }

        public void Visit(Repeat expression, Nonterminal context)
        {
        }

        public void Visit(EncloseExpression expression, Nonterminal context)
        {
            throw new NotImplementedException();
        }
    }
}