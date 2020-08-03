using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DOAdminAFIP.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> SplitBy(this string auxs, params char[] separators)
            => auxs.Split(separators);

        public static IEnumerable<string> SplitBy(this string auxs, params string[] separators)
            => auxs.Split(separators, StringSplitOptions.None);

        public static IEnumerable<IEnumerable<string>> SplitByTab(this IEnumerable<string> rows)
            => rows.Select(s => s.SplitBy(new char[] { '\t' }));

        public static IEnumerable<string[]> ToMatrix <T>(this IEnumerable<T> elements, Func<T, string[]> func)
        {
            foreach (var element in elements)
                yield return func(element);
        }

        public static string Concat<T>(this IEnumerable<T> arr) => arr.Aggregate(new StringBuilder(), (acum, s) => acum.Append(s)).ToString();
    }
}
