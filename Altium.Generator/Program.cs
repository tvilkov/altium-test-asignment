using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Altium.Generator
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                printUsage();
                return;
            }
            var numRecords = long.Parse(args[0]);
            var outputFile = args[1];

            var dictionaryFolder = Path.Combine(
                Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath),
                "dictionary");
            Generator.Vocabulary = Vocabulary.LoadFrom(dictionaryFolder);
            var generator = Generator.Build();

            var sw = new Stopwatch();
            sw.Start();
            generateCore(numRecords, 97, outputFile, generator);
            sw.Stop();
            Console.WriteLine(Environment.NewLine + "Elapsed: " + TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).ToString("g"));
        }

        private static void printUsage()
        {
            var exeName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            Console.WriteLine("Usage:\r\n\t{0} numberOfRecords outputFile", exeName);
        }

        private static void generateCore(long numRecords, int percentOfUnique, string outputFile, Func<string> generator)
        {
            var uniqueRecords = (long)Math.Round(0.01 * percentOfUnique * numRecords);
            var duplicateRecords = numRecords - uniqueRecords;
            var duplicates = new List<string>((int)Math.Min(1000, duplicateRecords));
            var rnd = new Random(DateTime.UtcNow.Second);
            using var fs = File.Create(outputFile);
            using var wr = new StreamWriter(fs, Encoding.ASCII, 32 * 1024 * 1024);
            while (uniqueRecords > 0 || duplicateRecords > 0)
            {
                var generateDuplicate = uniqueRecords <= 0 || (
                    // We haven't reached limit for duplicates yet
                    duplicateRecords > 0
                    // And we have duplicates to choose from
                    && duplicates.Count > duplicates.Capacity >> 1
                    // Probability 1/10
                    && rnd.Next(100) > 9);

                string str;
                var number = rnd.Next(int.MaxValue / 2);
                if (!generateDuplicate)
                {
                    str = generator();
                    if (duplicates.Count < duplicates.Capacity)
                    {
                        duplicates.Add(str);
                    }
                    --uniqueRecords;
                }
                else
                {
                    str = duplicates[rnd.Next(duplicates.Count)];
                    --duplicateRecords;
                }

                wr.Write(number);
                wr.Write(". ");
                wr.WriteLine(str);
            }
        }
    }
}
