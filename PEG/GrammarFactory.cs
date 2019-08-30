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
            factory.grammar = result;
            result.NotifyCreated(args);

            // Now initialize the grammar
            foreach (MethodInfo method in typeof(T).GetMethods())
            {
                if (method.ReturnType == typeof(Expression) && method.GetParameters().Length == 0 && !method.IsSpecialName)
                {
                    Nonterminal rule = new Nonterminal();
                    rule.Name = method.Name;
                    rule.Index = factory.nonterminalIndex++;
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

        private T grammar;
        private MethodInfo activeMethod;
        private Dictionary<string, Nonterminal> rules = new Dictionary<string, Nonterminal>();
        private int nonterminalIndex;

        /// <summary>
        /// The point of this is to evaluate all of the expressions and assign them to the rule (NonTerminal).  The
        /// "activeMethod" business is to escape recursion if a rule calls itself.
        /// </summary>
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

                    // Now check for captured nonterminals (i.e. .Capture("foo"))
                    var walker = new CaptureWalker();
                    rule.Expression.Accept(walker, this);

                    activeMethod = null;
                }
            }
        }

        /// <summary>
        /// Looks for captured nonterminals so we can give them an index and register them.
        /// </summary>
        private class CaptureWalker : ExpressionWalker<GrammarFactory<T>>
        {
            public override void Visit(Nonterminal expression, GrammarFactory<T> context)
            {
                // Captured nonterminals are given an index of -1 when created.
                if (expression.Index == -1)
                {
                    expression.Index = context.nonterminalIndex++;
                    context.grammar.Nonterminals.Add(expression);

                    // For these we still want to propagate the walk since there could be more nested content
                    base.Visit(expression, context);
                }
                else
                {
                    // We don't propagate the walk since we don't want to follow into any other nonterminals
                }
            }
        }
    }
}