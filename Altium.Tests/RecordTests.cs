using System;
using System.Collections.Generic;
using Altium.Sorter;
using NUnit;
using NUnit.Framework;

namespace Altium.Tests
{
    [TestFixture]
    internal class RecordTests
    {
        [Test]
        public void CtorTest()
        {
            var rec = new Record(1, "Test");
            Assert.AreEqual(1, rec.Number);
            Assert.AreEqual("Test", rec.String);
        }

        [Test]
        public void ToStringTest()
        {
            var rec = new Record(123, "Test");
            Assert.AreEqual("123. Test", rec.ToString());
        }

        [TestCase("1. Test", 1L, "Test")]
        [TestCase("1234567890. Test", 1234567890L, "Test")]
        [TestCase("123. Multiple words here", 123L, "Multiple words here")]
        [TestCase("1. Non Trimmed End ", 1L, "Non Trimmed End ")]
        [TestCase("1.  Non Trimmed Start", 1L, " Non Trimmed Start")]
        public void ParsePositiveTests(string input, long expectedNumber, string expectedString)
        {
            var rec = Record.Parse(input);
            Assert.AreEqual(expectedNumber, rec.Number);
            Assert.AreEqual(expectedString, rec.String);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("\r\n")]
        [TestCase("123")]
        [TestCase("String")]
        [TestCase("123String")]
        [TestCase("123;String")]
        [TestCase(".String")]
        [TestCase("abc.String")]
        [TestCase("-1.String")]
        [TestCase(" 1.String")]
        [TestCase("1 .String")]
        public void ParseNegativeTests(string input)
        {
            Assert.Throws<FormatException>(() => Record.Parse(input));
        }

        internal class RecordComparerTests
        {
            [Test]
            [TestCaseSource(nameof(TestCases))]
            public void CompareTest(Record left, Record right, int expectedCmpResult)
            {
                Assert.AreEqual(expectedCmpResult, Record.DefaultComparer.Compare(left, right));
            }

            public static IEnumerable<TestCaseData> TestCases()
            {
                yield return new TestCaseData(new Record(1, "Test"), new Record(1, "Test"), 0);
                yield return new TestCaseData(new Record(123, "Test Me"), new Record(123, "Test Me"), 0);
                yield return new TestCaseData(new Record(1, "abc"), new Record(2, "abc"), -1);
                yield return new TestCaseData(new Record(1, "abc"), new Record(1, "bcd"), -1);
                yield return new TestCaseData(new Record(1, "bcd"), new Record(2, "abc"), 1);
                yield return new TestCaseData(new Record(2, "bcd"), new Record(1, "bcd"), 1);
            }
        }
    }
}
