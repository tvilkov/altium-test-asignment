using System;
using System.Collections.Generic;

namespace Altium.Sorter
{
    internal static class CollectionExtensions
    {
        public static IEnumerable<T> MergeSorted<T>(this IEnumerable<IEnumerable<T>> sortedSources, IComparer<T> comparer)
        {
            if (sortedSources == null) throw new ArgumentNullException(nameof(sortedSources));
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            var readers = new SortedList<T, IEnumerator<T>>(comparer);
            foreach (var source in sortedSources)
            {
                var en = source.GetEnumerator();
                if (!en.MoveNext())
                {
                    en.Dispose();
                    continue;
                }
                readers.Add(en.Current, en);
            }

            while (readers.Count > 0)
            {
                var min = readers.Values[0];
                yield return min.Current;

                readers.RemoveAt(0);
                if (min.MoveNext())
                {
                    readers.Add(min.Current, min);
                }
                else
                {
                    min.Dispose();
                }
            }
        }
    }
}