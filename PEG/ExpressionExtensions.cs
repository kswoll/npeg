using System.Collections.Generic;
using System.Linq;
using PEG.Cst;
using PEG.Extensions;
using PEG.SyntaxTree;

namespace PEG
{
    public static class ExpressionExtensions
    {
        public static CharacterSet To(this char from, char to)
        {
            if (from == '(' && to == ')')
            {
                char[] symbols =
                {
                    '-', '\'', '\\', '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '=', '+', '[',
                    ']', '{', '}', '|', ';', ':', '\"', '<', '>', ',', '.', '/', '?'
                };
                return new CharacterRange(from, to, symbols.Select(x => (CharacterTerminal)x));
            }
            else if ((from == 'A' && to == 'z') || (from == 'a' && to == 'Z'))
            {
                return new CharacterRange(from, to, from.RangeTo(to).Select(x => (CharacterTerminal)x));
            }
            else
            {
                return new CharacterRange(from, to, from.RangeTo(to).Select(x => (CharacterTerminal)x).ToArray());
            }
        }

        /// <summary>
        /// Returns true if the pattern matches the specified input.  This requires an exact
        /// match, contrasted against Contains(...).
        /// </summary>
        public static bool Match(this Expression expression, string input)
        {
            var parser = CreateParser(expression);
            var result = parser.ParseString(input);
            return result != null && result.Any();
        }

        /// <summary>
        /// Returns true if the pattern is contained by the specified input.  This performs
        /// a partial match.  If the pattern is found anywhere in the specifieed input,
        /// this returns true.
        /// </summary>
        public static bool Contains(this Expression expression, string input)
        {
            var exp = +(expression | new AnyCharacter(false));
            var parser = CreateParser(exp);
            IEnumerable<OutputRecord> outputRecords = parser.ParseString(input);
            return outputRecords.Skip(2).Any();     // Skip begin/end of implicit root nonterminal
        }

        /// <summary>
        /// PEG requires that the nonterminals be indexed in so that they may be accessed
        /// quickly from an array.  Compiling the expression will ensure all nonterminals
        /// contained in the expression (and its descendents) are properly indexed.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static void Compile(this Expression expression)
        {
            var compiler = new ExpressionCompiler();
            expression.Accept(compiler, null);
        }

        /// <summary>
        /// Performs a transformation on the input substituting the values as defined by the provided
        /// replacements.  This requires an exact match similar to the Match(...) method.
        /// </summary>
        public static string Transform(this Expression expression, string input, params Replacement[] replacements)
        {
            var parser = CreateParser(expression);

            var output = parser.ParseString(input);
            if (!output.Any())
                return input;

            var cst = CstBuilder.Build(output);
            cst = (CstNonterminalNode)cst.Children[0];
            var cache = new CstCache(cst);
            var map = replacements.ToDictionary(x => x.From, x => x.To.Replace(cache));
            var transformed = cst.Transform(x => map.ContainsKey(x.Nonterminal) ? map[x.Nonterminal] : null);

            return transformed.Coalesce();
        }

        /// <summary>
        /// Replaces all occurances of the pattern within input according to the definition provided
        /// by replacements.
        /// </summary>
        public static string Replace(this Expression expression, string input, params Replacement[] replacements)
        {
            // We want the expression to act like a nonterminal so that its contents get grouped up in the CST
            if (!(expression is Nonterminal))
                expression = expression.Capture();

            // Modify the expression to allow any character (allows matching substrings)
            var exp = +(expression | new AnyCharacter(true));

            var parser = CreateParser(exp);

            var output = parser.ParseString(input);
            if (!output.Any())
                return input;

            var cst = CstBuilder.Build(output);
            cst = (CstNonterminalNode)cst.Children[0];

            var transformedCst = new CstNonterminalNode(cst.Nonterminal, -1);
            foreach (var child in cst.Children)
            {
                if (child is CstNonterminalNode)
                {
                    var childNonterminal = (CstNonterminalNode)child;
                    var cache = new CstCache(childNonterminal);
                    var map = replacements.ToDictionary(x => x.From, x => x.To.Replace(cache));
                    var transformed = childNonterminal.Transform(x => map.ContainsKey(x.Nonterminal) ? map[x.Nonterminal] : null);
                    transformedCst.Children.Add(transformed);
                }
                else
                {
                    transformedCst.Children.Add(child);
                }
            }

            return transformedCst.Coalesce();
        }

        public static PegParser CreateParser(Expression expression)
        {
            Grammar grammar = new Grammar();
            var nonterminal = expression is Nonterminal ? (Nonterminal)expression : new Nonterminal(expression);
            grammar.Nonterminals.AddRange(nonterminal.Enumerate().OfType<Nonterminal>());

            // Compile is necessary to ensure nonterminals have valid indices
            nonterminal.Compile();

            PegParser parser = new PegParser(grammar, nonterminal);
            return parser;
        }

        public static Dictionary<Nonterminal, string> Parse(this Expression expression, string input)
        {
            var parser = CreateParser(expression);
            var output = CstBuilder.Build(parser.ParseString(input));
            if (output == null)
                return null;

            var dictionary = new Dictionary<Nonterminal, string>();
            foreach (var nonterminal in output.FindAllNonterminalNodes())
                if (nonterminal.Nonterminal != null)
                    dictionary[nonterminal.Nonterminal] = nonterminal.Coalesce();

            return dictionary;
        }

        public static IEnumerable<Expression> Enumerate(this Expression expression)
        {
            return new ExpressionEnumerator(expression);
        }

        /// <summary>
        /// Shortcut for character terminals.  'a'._() converts 'a' into the CharacterTerminal
        /// expression representing the character 'a'.  This is a common operation and this seems
        /// like the most concise of representing it.
        /// </summary>
        public static CharacterTerminal _(this char c)
        {
            return new CharacterTerminal(c);
        }

        public static Sequence _(this string s)
        {
            return s;
        }

        /// <summary>
        /// Returns a Nonterminal that references the current expression, giving it the specified
        /// name.  Nonterminals are the only structure that retains structural meaning after
        /// parsing (i.e. it has representation in the output stream / CST.)  Thus, if you ever
        /// need to do something to your parse result by referring to elements in your pattern,
        /// this is how you can solidify them.
        /// </summary>
        public static Nonterminal Capture(this Expression expression, string name)
        {
            Nonterminal result = new Nonterminal();
            result.Name = name;
            result.Expression = expression;
            result.Index = -1;
            return result;
        }

        /// <summary>
        /// Returns a Nonterminal that references the current expression.  Nonterminals are
        /// the only structure that retains structural meaning after parsing (i.e. it
        /// has representation in the output stream / CST.)  Thus, if you ever need to do
        /// something to your parse result by referring to elements in your pattern,
        /// this is how you can solidify them.
        /// </summary>
        public static Nonterminal Capture(this Expression expression)
        {
            Nonterminal result = new Nonterminal();
            result.Expression = expression;
            result.Index = -1;
            return result;
        }

        public static Replacement To(this Nonterminal from, Nonterminal to)
        {
            return new Replacement { From = from, To = new NonterminalReplacementTarget(to) };
        }

        public static Replacement To(this Nonterminal from, string to)
        {
            return new Replacement { From = from, To = new StringReplacementTarget(to) };
        }

        public static Expression Repeat(this Expression expression, int count)
        {
            Repeat repeat = new Repeat();
            repeat.Operand = expression;
            repeat.Min = count;
            repeat.Max = count;
            return repeat;
        }

        public static Expression Repeat(this Expression expression, int min, int max)
        {
            Repeat repeat = new Repeat();
            repeat.Operand = expression;
            repeat.Min = min;
            repeat.Max = max;
            return repeat;
        }

        public static AndPredicate And(this Expression operand)
        {
            AndPredicate andPredicate = new AndPredicate();
            andPredicate.Operand = operand;
            return andPredicate;
        }

        public static EncloseExpression Enclose(this char enclosure, Expression operand)
        {
            return enclosure._().Enclose(operand);
        }

        public static EncloseExpression Enclose(this string enclosure, Expression operand)
        {
            return enclosure._().Enclose(operand);
        }

        public static EncloseExpression Enclose(this Expression enclosure, Expression operand)
        {
            EncloseExpression enclose = new EncloseExpression();
            enclose.Enclosure = enclosure;
            enclose.Operand = operand;
            return enclose;
        }

        public static AnyCharacter Any(this char c)
        {
            return new AnyCharacter();
        }
    }
}