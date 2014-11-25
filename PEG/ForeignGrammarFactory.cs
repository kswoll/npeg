using System.Collections.Generic;
using System.Reflection;
using PEG.SyntaxTree;

namespace PEG
{
    /// <summary>
    /// Facilitates tokenization by allowing you to have a reference to a foreign tokenization grammar.
    /// The values returned by this grammar are instances of ForeignNonterminal.
    /// </summary>
    public class ForeignGrammarFactory<T> : IInterceptor where T : Grammar<T>
    {
        private static ProxyGenerator proxyGenerator = new ProxyGenerator();

        public static T Create()
        {
            ForeignGrammarFactory<T> factory = new ForeignGrammarFactory<T>();
            T result = proxyGenerator.CreateClassProxy<T>(factory);

            int nonterminalIndex = 0;

            // Now initialize the grammar
            foreach (MethodInfo method in typeof(T).GetMethods())
            {
                if (method.ReturnType == typeof(Expression) && method.GetParameters().Length == 0 && !method.IsSpecialName)
                {
                    ForeignNonterminal rule = new ForeignNonterminal();
                    rule.Name = method.Name;
                    factory.rules[rule.Name] = rule;
                }
            }

            T tokenizerGrammar = GrammarFactory<T>.Create();
            result.SetTokenizer(tokenizerGrammar, tokenizerGrammar.Tokenizer);

            return result;
        }

        private Dictionary<string, ForeignNonterminal> rules = new Dictionary<string, ForeignNonterminal>();

        public void Intercept(IInvocation invocation)
        {
            ForeignNonterminal result;
            if (rules.TryGetValue(invocation.Method.Name, out result))
                invocation.ReturnValue = result;
            else
            {
                invocation.Proceed();
            }
        }        
    }
}