using System.IO;
using System.Linq;
using Altium.Sorter;
using NUnit.Framework;

namespace Altium.Tests
{
    [TestFixture]
    internal class RecordIOTests
    {
        [Test]
        public void ReadFileTest()
        {
            var records = RecordIO.ReadFile(DataFile("input.txt")).ToArray();
            Assert.AreEqual(5, records.Length);
            Assert.AreEqual(415, records[0].Number);
            Assert.AreEqual("Apple", records[0].String);
            Assert.AreEqual(2, records[^1].Number);
            Assert.AreEqual("Banana is yellow", records[^1].String);
        }

        [Test]
        public void ChunkReadFileTest()
        {
            var chunks = RecordIO.ChunkReadFile(DataFile("input.txt"), 2).ToArray();

            Assert.AreEqual(3, chunks.Length);
            Assert.AreEqual(2, chunks[0].Length);
            Assert.AreEqual(2, chunks[1].Length);
            Assert.AreEqual(1, chunks[2].Length);
        }

        [Test]
        public void WriteFileTest()
        {
            var file = DataFile("write-test.txt");
            try
            {
                var records = new[]
                {
                    new Record(1, "Number one"),
                    new Record(2, "Number two"),
                    new Record(3, "Number three"),
                };
                RecordIO.WriteFile(file, records);

                var content = File.ReadAllText(file);
                foreach (var rec in records)
                {
                    StringAssert.Contains(rec.ToString(), content);
                }
            }
            finally
            {
                File.Delete(file);
            }
        }

        protected static string DataFile(string name)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "TestData", name);
        }
    }
}
