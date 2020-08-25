using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Altium.Sorter
{
    internal static class RecordIO
    {
        public static IEnumerable<Record> ReadFile(string file)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));

            using var reader = File.OpenText(file);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                yield return Record.Parse(line);
            }
        }

        public static IEnumerable<Record[]> ChunkReadFile(string file, int chunkSize)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (chunkSize <= 0) throw new ArgumentOutOfRangeException(nameof(chunkSize));

            Record[] buffer = null;
            var read = 0;
            foreach (var rec in ReadFile(file))
            {
                if (buffer == null)
                {
                    buffer = new Record[chunkSize];
                    read = 0;
                }

                buffer[read++] = rec;
                if (buffer.Length == read)
                {
                    yield return buffer;
                    buffer = null;
                }
            }

            // Last chunk
            if (buffer != null)
            {
                Array.Resize(ref buffer, read);
                yield return buffer;
            }
        }

        public static void WriteFile(string file, IEnumerable<Record> items)
        {
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (items == null) throw new ArgumentNullException(nameof(items));

            using var wr = new StreamWriter(file);
            foreach (var rec in items)
            {
                wr.WriteLine(rec.ToString());
            }
        }

        public static void MergeFiles(IEnumerable<string> files, string outFile)
        {
            if (files == null) throw new ArgumentNullException(nameof(files));
            if (outFile == null) throw new ArgumentNullException(nameof(outFile));

            using var wr = File.CreateText(outFile);
            var merged = files.Select(ReadFile).MergeSorted(Record.DefaultComparer);
            foreach (var rec in merged)
            {
                wr.WriteLine(rec.ToString());
            }
        }
    }
}