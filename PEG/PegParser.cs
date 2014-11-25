using System;
using System.Collections.Generic;
using System.Linq;
using PEG.Builder;
using PEG.SyntaxTree;

namespace PEG
{
    public class PegParser
    {
        public Grammar Grammar { get; set; }
        public Expression StartExpression { get; set; }

        public PegParser(Grammar grammar)
        {
            Grammar = grammar;
        }

        public PegParser(Grammar grammar, Expression startExpression)
        {
            Grammar = grammar;
            StartExpression = startExpression;
        }

        public IEnumerable<OutputRecord> ParseString(string input)
        {
            LrParseEngine parseEngine = new LrParseEngine(Grammar, input);
            Expression startExpression = StartExpression ?? Grammar.StartExpression;
            IEnumerable<OutputRecord> outputRecords = startExpression.Execute(parseEngine);
            if (outputRecords != null && outputRecords.Any() && parseEngine.Position < input.Length)
                outputRecords = Enumerable.Empty<OutputRecord>();
            return outputRecords;
        }
    }

    public class PegParser<T> : PegParser
    {
        public PegBuilder<T> Builder { get; set; }

        public PegParser(Grammar grammar) : this(grammar, grammar.Nonterminals[0])
        {
        }

        public PegParser(Grammar grammar, Expression startExpression) : base(grammar, startExpression)
        {
            Builder = new PegBuilder<T>(Grammar);
        }

        public PegParser(Grammar grammar, Expression startExpression, PegBuilder<T> builder) : base(grammar, startExpression)
        {
            Builder = builder;
        }

        public T Parse(string input)
        {
            int amountRead;
            return Parse(input, out amountRead);
        }

        public T Parse(string input, out int amountRead)
        {
            LrParseEngine parseEngine = CreateParseEngine(input);
            Expression startExpression = StartExpression ?? Grammar.StartExpression;
            T result = Parse(parseEngine, startExpression);
            amountRead = parseEngine.Position;
            return result;
        }

        public bool Parse(string input, out T result, out int amountRead)
        {
            LrParseEngine parseEngine = CreateParseEngine(input);
            Expression startExpression = StartExpression ?? Grammar.StartExpression;
            bool returnValue = Parse(parseEngine, startExpression, out result);
            amountRead = returnValue ? parseEngine.Position : 0;
            return returnValue;
        }

        private T Parse(LrParseEngine parseEngine, Expression startExpression)
        {
            IEnumerable<OutputRecord> outputStream = startExpression.Execute(parseEngine);
            if (!parseEngine.IsFailure(outputStream))
            {
                T result = Builder.Build(outputStream);
                return result;
            }
            else
            {
                throw parseEngine.CreateException();
            }
        }

        private bool Parse(LrParseEngine parseEngine, Expression startExpression, out T result)
        {
            IEnumerable<OutputRecord> outputStream = startExpression.Execute(parseEngine);
            if (!parseEngine.IsFailure(outputStream))
            {
                result = new PegBuilder<T>(Grammar).Build(outputStream);
                return true;
            }
            else
            {
                result = default(T);
                return false;
            }
        }

        private LrParseEngine CreateParseEngine(string input)
        {
            LrParseEngine parseEngine;
            if (Grammar.Tokenizer != null) 
            {
                parseEngine = Tokenize(input);
            }
            else
            {
                parseEngine = new LrParseEngine(Grammar, input);
            }
            return parseEngine;
        }

        private LrParseEngine Tokenize(string input)
        {
            LrParseEngine tokenizerEngine = new LrParseEngine(Grammar.TokenizerGrammar.TokenizerGrammar, input);
            Nonterminal tokenizer = Grammar.Tokenizer;
            List<Terminal> tokenOutput = new List<Terminal>();
            int absoluteIndex = 0;
            while (tokenizerEngine.Position < input.Length)
            {
                int position = tokenizerEngine.Position;
                IEnumerable<OutputRecord> output = tokenizer.Execute(tokenizerEngine);
                if (output != null)
                {
                    Nonterminal rule = output.Select(o => o.Expression).OfType<Nonterminal>().Where(o => o.IsToken).First();
                    if (!rule.IsTokenOmitted)
                    {
                        // Trim tokenizer
                        output = TrimOutput(output);

                        Token token = new Token(rule, output, absoluteIndex++);
                        token.Position = position;
                        tokenOutput.Add(token);
                    }
                }
                // If the tokenization iteration did not move the position forward at all,
                // then the current character is not part of a valid token.  Rather than
                // considering this an error, we will just add the character into the next
                // input stream as-is -- as a CharacterTerminal.  This is useful for tokens
                // that are context sensitive.  An example is the C# shift-right operator 
                // together with closing two generic types at once.  Both will make use of 
                // the character sequence ">>".  However, in the case of the shift-right 
                // operation, the "token" should be the full "<<" -- both characters.  On 
                // the other hand, when closing a generic type, the token should be a single 
                // '>' (followed in this case by another '>' immediately after.)  The best way
                // to disambiguate this situation is to not create a token and allow the full
                // grammar to treat the actual characters (hence the full grammar will actually
                // contain raw characters whereas a grammar dependent on a fully tokenized (as
                // opposed to partially tokenized) input would not have any terminals in its
                // grammar except foreign references to the tokens defined in the token grammar.
                if (tokenizerEngine.Position == position)
                {
                    tokenOutput.Add(new CharacterTerminal(input[position]));
                    if (!tokenizerEngine.Consume())
                        throw new InvalidOperationException("Not sure what to do if this happens");
                }
            }
            return new LrParseEngine(Grammar, input, new TokenParseInput(tokenOutput.ToArray()));
        }

        private IEnumerable<OutputRecord> TrimOutput(IEnumerable<OutputRecord> source)
        {
            var enumerator = source.GetEnumerator();
            enumerator.MoveNext();  // Now at beginning
            enumerator.MoveNext();  // Now past first entry

            OutputRecord last = null;
            OutputRecord secondLast = null;
            while (enumerator.MoveNext())
            {
                if (secondLast != null)
                    yield return secondLast;
                secondLast = last;
                last = enumerator.Current;
            }
        }
    }
}