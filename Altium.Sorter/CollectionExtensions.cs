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

            var readers = new List<IEnumerator<T>>();
            foreach (var source in sortedSources)
            {
                var en = source.GetEnumerator();
                if (!en.MoveNext())
                {
                    en.Dispose();
                    continue;
                }
                readers.Add(en);
            }

            while (readers.Count > 0)
            {
                var min = readers[0];
                for (var i = 1; i < readers.Count; ++i)
                {
                    if (comparer.Compare(readers[i].Current, min.Current) < 0)
                    {
                        min = readers[i];
                    }
                }
                yield return min.Current;
                if (!min.MoveNext())
                {
                    min.Dispose();
                    readers.Remove(min);
                }
            }
        }
    }
}