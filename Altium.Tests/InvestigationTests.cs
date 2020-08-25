using System;
using System.Diagnostics;
using System.Linq;
using Altium.Sorter;
using NUnit.Framework;

namespace Altium.Tests
{
    [TestFixture, Ignore("")]
    public class InvestigationTests
    {
        [TestCase("200Mb")] // 2sec
        [TestCase("500Mb")] // 4sec
        [TestCase("1Gb")]   // 12sec
        [TestCase("10Gb")]  // 1min 20sec
        public void ParseFile(string size)
        {
            var file = $@"D:\tmp\{size}.txt";
            Measure(() =>
            {
                foreach (var _ in RecordIO.ReadFile(file))
                {
                }
            });
        }

        [TestCase("200Mb")] // 6sec
        [TestCase("500Mb")] // 17sec (!)
        [TestCase("1Gb")]   // 51sec (!!)
        // [TestCase("10Gb")]
        public void SortInMemory(string size)
        {
            var file = $@"D:\tmp\{size}.txt";
            var data = RecordIO.ReadFile(file).ToArray();
            Measure(() =>
            {
                Array.Sort(data, Record.DefaultComparer);
            });
        }

        protected void Measure(Action action)
        {
            var sw = new Stopwatch();
            sw.Start();
            action();
            sw.Stop();
            Console.WriteLine(TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds));
        }
    }
}