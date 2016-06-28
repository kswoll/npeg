using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PEG.Extensions;
using PEG.Proxies;
using PEG.SyntaxTree;

namespace PEG
{
    public class GrammarFactory<T> where T : Grammar<T>
    {
        public static T Create(object[] args)
        {
            GrammarFactory<T> factory = new GrammarFactory<T>();
            T result = Proxy.CreateProxy<T>(factory.InvocationHandler);
            result.NotifyCreated(args);

            int nonterminalIndex = 0;

            // Now initialize the grammar
            foreach (MethodInfo method in typeof(T).GetMethods())
            {
                if (method.ReturnType == typeof(Expression) && method.GetParameters().Length == 0 && !method.IsSpecialName)
                {
                    Nonterminal rule = new Nonterminal();
                    rule.Name = method.Name;
                    rule.Index = nonterminalIndex++;
                    result.Nonterminals.Add(rule);

//                    if (method.HasAttribute<StartRuleAttribute>())
//                        result.StartRule = rule;

                    factory.rules[method.Name] = rule;
                }
            }
            foreach (MethodInfo method in typeof(T).GetMethods())
            {
                if (method.ReturnType == typeof(Expression) && method.GetParameters().Length == 0 && !method.IsSpecialName)
                {
                    method.Invoke(result, null);
                }
            }
            foreach (MethodInfo method in typeof(T).GetMethods())
            {
                if (method.ReturnType == typeof(Expression) && method.GetParameters().Length == 0 && !method.IsSpecialName)
                {
                    Nonterminal rule = factory.rules[method.Name];
                    if (method.HasAttribute<TokenizerAttribute>())
                    {
                        result.Tokenizer = rule;
                    }
                    TokenAttribute tokenAttribute = method.GetAttribute<TokenAttribute>();
                    if (tokenAttribute != null)
                    {
                        if (!tokenAttribute.TokenizeChildren)
                        {
                            rule.IsToken = true;
                            if (tokenAttribute.IsOmitted)
                                rule.IsTokenOmitted = true;
                        }
                        else
                        {
                            OrderedChoice children = (OrderedChoice)rule.Expression;
                            foreach (var child in children.Expressions)
                            {
                                Nonterminal childRule = (Nonterminal)child;
                                childRule.IsToken = true;
                            }
                        }
                    }
                }
            }

            FieldInfo tokenizerGrammarField = typeof(T).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(o => typeof(Grammar).IsAssignableFrom(o.FieldType) && o.HasAttribute<TokenizerAttribute>()).FirstOrDefault();
            if (tokenizerGrammarField != null)
            {
                Grammar tokenizerGrammar = (Grammar)tokenizerGrammarField.GetValue(result);
                if (tokenizerGrammar.Tokenizer != null)
                    result.SetTokenizer(tokenizerGrammar, tokenizerGrammar.Tokenizer);
            }

            return result;
        }

        private MethodInfo activeMethod;
        private Dictionary<string, Nonterminal> rules = new Dictionary<string, Nonterminal>();

        private void InvocationHandler(Invocation invocation)
        {
            Nonterminal result;
            if (rules.TryGetValue(invocation.Method.Name, out result) && result.Expression != null)
                invocation.ReturnValue = result;
            else
            {
                if (activeMethod != null)
                {
                    Nonterminal rule = rules[invocation.Method.Name];
                    invocation.ReturnValue = rule;
                }
                else
                {
                    activeMethod = invocation.Method;
                    invocation.Proceed();
                    Nonterminal rule = rules[invocation.Method.Name];
                    if (rule.Expression == null)
                        rule.Expression = (Expression)invocation.ReturnValue;
                    invocation.ReturnValue = rule;
                    activeMethod = null;
                }
            }
        }
    }
}