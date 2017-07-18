using System.Collections.Generic;
using System.Reflection;
using PEG.Proxies;
using PEG.SyntaxTree;

namespace PEG
{
    public class GrammarFactory<T> where T : Grammar<T>
    {
        private readonly Dictionary<string, Nonterminal> rules = new Dictionary<string, Nonterminal>();

        private MethodInfo activeMethod;

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

            return result;
        }

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