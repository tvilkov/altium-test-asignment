using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Altium.Tests")]

namespace Altium.Sorter
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                printUsage();
                return;
            }

            var inFile = args[0];
            if (!File.Exists(inFile))
            {
                Console.WriteLine($"File {inFile} not exists");
                return;
            }

            var outFile = args[1];

            var sw = new Stopwatch();
            sw.Start();
            var sort = new DataFlowSort(Console.WriteLine);
            sort.Sort(inFile, outFile);
            sw.Stop();
            Console.WriteLine("Elapsed: " + TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds).ToString("g"));
            Console.ReadLine();
        }

        private static void printUsage()
        {
            var exeName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            Console.WriteLine("Usage:\r\n\t{0} inFile outFile", exeName);
        }
    }
}
