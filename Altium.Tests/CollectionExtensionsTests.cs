using System;
using System.Linq;
using Altium.Sorter;
using NUnit.Framework;

namespace Altium.Tests
{
    [TestFixture]
    internal class CollectionExtensionsTests
    {
        [TestFixture]
        public class MergeTests
        {
            [Test]
            public void MergeEmptyBoth()
            {
                var left = Enumerable.Empty<Record>();
                var right = Enumerable.Empty<Record>();
                var merged = new[] { left, right }.MergeSorted(Record.DefaultComparer);
                CollectionAssert.IsEmpty(merged);
            }

            [Test]
            public void MergeEmptyLeft()
            {
                var left = Enumerable.Empty<Record>();
                var right = new[]
                {
                    new Record(1, "One"),
                    new Record(2, "Two"),
                };
                var merged = new[] { left, right }.MergeSorted(Record.DefaultComparer);
                CollectionAssert.AreEqual(merged, right);
            }

            [Test]
            public void MergeEmptyRight()
            {
                var left = new[]
                {
                    new Record(1, "One"),
                    new Record(2, "Two"),
                };
                var right = Enumerable.Empty<Record>();
                var merged = new[] { left, right }.MergeSorted(Record.DefaultComparer);
                CollectionAssert.AreEqual(merged, left);
            }

            [Test]
            public void MergeNonEmpty()
            {
                var left = new[]
                {
                    new Record(1, "A"),
                    new Record(1, "C"),
                    new Record(1, "E"),
                    new Record(3, "E")
                };
                var right = new[]
                {
                    new Record(1, "B"),
                    new Record(1, "D"),
                    new Record(2, "E"),
                };
                var expected = left.Concat(right).ToArray();
                Array.Sort(expected, Record.DefaultComparer);
                var merged = new[] { left, right }.MergeSorted(Record.DefaultComparer);
                CollectionAssert.AreEqual(expected, merged);
            }
        }
    }
}
