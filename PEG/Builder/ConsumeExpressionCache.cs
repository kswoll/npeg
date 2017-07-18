using System.Collections.Generic;

namespace PEG.Builder
{
    public class ConsumeExpressionCache
    {
        private static readonly Dictionary<string, ConsumeExpression> cache = new Dictionary<string, ConsumeExpression>();
        private static readonly object lockObject = new object();

        public static ConsumeExpression Get(string expression)
        {
            ConsumeExpression result;
            bool found;
            lock (lockObject)
            {
                found = cache.TryGetValue(expression, out result);
            }
            if (!found)
            {
                result = ConsumeExpressionParsing.Parse(expression);
                lock (lockObject)
                {
                    cache[expression] = result;
                }
            }
            return result;
        }
    }
}