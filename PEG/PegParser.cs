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

        public (ParseOutput output, ParseOutputSpan span) ParseString(string input, bool requireCompleteParse = true)
        {
            LrParseEngine parseEngine = new LrParseEngine(Grammar, input);
            Expression startExpression = StartExpression ?? Grammar.StartExpression;
            var outputRecords = startExpression.Execute(parseEngine);
            if (!outputRecords.IsFailed && ((outputRecords.Any && parseEngine.Position < input.Length && requireCompleteParse) || !outputRecords.Any))
                outputRecords = parseEngine.False;
            return (parseEngine.Output, outputRecords);
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
            var outputStream = startExpression.Execute(parseEngine);
            if (!outputStream.IsFailed)
            {
                var result = Builder.Build(parseEngine.Output, outputStream);
                return result;
            }
            else
            {
                throw parseEngine.CreateException();
            }
        }

        private bool Parse(LrParseEngine parseEngine, Expression startExpression, out T result)
        {
            var outputStream = startExpression.Execute(parseEngine);
            if (!outputStream.IsFailed)
            {
                result = new PegBuilder<T>(Grammar).Build(parseEngine.Output, outputStream);
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
            LrParseEngine parseEngine = new LrParseEngine(Grammar, input);
            return parseEngine;
        }
    }
}