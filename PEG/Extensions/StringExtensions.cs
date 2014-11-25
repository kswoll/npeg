using System;
using System.Collections.Generic;
using System.Text;

namespace PEG.Extensions
{
    public static class StringExtensions
    {
        public static string GetLine(this string s, int offset, out int lineOffset)
        {
            if (offset >= s.Length)
            {
                string result = GetLine(s, s.Length - 1, out lineOffset);
                lineOffset = result.Length + (offset - s.Length) + 1;
                return result;
            }

            int start = s.LastIndexOfAny(new[] { '\n', '\r' }, offset);
            if (start == -1)
                start = 0;
            else if (start < offset)    // If start == offset, then the offset is at a line break.   The code below will adjust for that and get the previous line
                start++;

            int end = s.IndexOfAny(new[] { '\n', '\r' }, offset);
            if (end == -1)
                end = s.Length;

            if (start == end && offset > 0)
            {
                string result = GetLine(s, offset - 1, out lineOffset);
                lineOffset = result.Length + 1;
                return result;
            }

            lineOffset = offset - start;
            return s.Substring(start, end - start);
        }

        public static string Delimit<T>(this IEnumerable<T> list, string delimiter)
        {
            if (list == null)
                return "";
            return Delimit(list, delimiter, o => o != null ? o.ToString() : "");
        }

        public static string Delimit<T>(this IEnumerable<T> list, string delimiter, Func<T, string> toString)
        {
            StringBuilder result = new StringBuilder();
            Delimit(list, t => result.Append(toString(t)), () => result.Append(delimiter));
            return result.ToString();
        }

        public static void Delimit<T>(this IEnumerable<T> list, Action<T> action, Action delimiterAction)
        {
            bool first = true;
            foreach (T o in list)
            {
                if (!first)
                    delimiterAction();
                action(o);
                first = false;
            }
        }
    }
}