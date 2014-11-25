using System;
using System.Collections.Generic;

namespace PEG.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Union<T>(this IEnumerable<T> enumerable, T element)
        {
            foreach (T t in enumerable)
                yield return t;
            yield return element;
        }

        public static void Foreach<T>(this IEnumerable<T> list, Action<T> action)
        {
            foreach (T o in list)
                action(o);
        }

        public static void Foreach<T, TResult>(this IEnumerable<T> list, Func<T, TResult> action)
        {
            foreach (T o in list)
                action(o);
        }
    }
}