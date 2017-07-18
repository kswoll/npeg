using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PEG.SyntaxTree;
using Expression = PEG.SyntaxTree.Expression;

namespace PEG
{
    public class Grammar
    {
        public static EmptyString Empty = new EmptyString();
        public static AnyCharacter Any = new AnyCharacter();

        public List<Nonterminal> Nonterminals { get; set; }
        public Expression StartExpression { get; set; }
        public Grammar TokenizerGrammar { get; private set; }
        public Nonterminal Tokenizer { get; set; }
        public bool IsTokenGrammar { get; private set; }

//        public FirstSet FirstSet { get; set; }

        public Grammar()
        {
            Nonterminals = new List<Nonterminal>();
        }

        public void SetTokenizer(Grammar tokenizerGrammar, Nonterminal tokenizer)
        {
            TokenizerGrammar = tokenizerGrammar;
            Tokenizer = tokenizer;
            tokenizerGrammar.IsTokenGrammar = true;
        }

        public Nonterminal GetNonterminal(string name)
        {
            return Nonterminals.FirstOrDefault(o => o.Name == name);
        }
    }

    public class Grammar<T> : Grammar where T : Grammar<T>
    {
        public static T Create(params object[] args)
        {
            Grammar<T> grammar = GrammarFactory<T>.Create(args);
            grammar.Initialize();
            return (T)grammar;
        }

        internal void NotifyCreated(object[] args)
        {
            OnCreated(args);
        }

        protected virtual void OnCreated(object[] args)
        {
        }

        public void Initialize()
        {
            var missingVirtual = typeof(T).GetMethods().Where(x => x.DeclaringType != typeof(Expression) && x.DeclaringType != typeof(Grammar) && x.ReturnType == typeof(Expression) && !x.IsVirtual).ToArray();
            if (missingVirtual.Any())
                throw new InvalidOperationException("The expression methods in your grammar must be declared virtual: " + string.Join(", ", missingVirtual.Select(x => x.Name)));
        }

        public Nonterminal GetNonterminal(Expression<Func<T, Expression>> nonterminal)
        {
            MethodCallExpression method = (MethodCallExpression)nonterminal.Body;
            return GetNonterminal(method.Method.Name);
        }
    }
}