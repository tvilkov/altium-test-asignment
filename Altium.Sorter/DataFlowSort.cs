using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace Altium.Sorter
{
    internal class DataFlowSort : ISort
    {
        private readonly Action<string> m_Logger;

        public DataFlowSort(Action<string> logger)
        {
            m_Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected void Log(string message) => m_Logger(message);

        public void Sort(string inFile, string outFile)
        {
            if (string.IsNullOrWhiteSpace(inFile)) throw new ArgumentNullException(nameof(inFile));
            if (string.IsNullOrWhiteSpace(outFile)) throw new ArgumentNullException(nameof(outFile));

            var fi = new FileInfo(inFile);
            var tmpFileNameFormat = Path.Combine(fi.DirectoryName, fi.Name + "~{0:D5}");
            const int mergeBatchSize = 10;
            //  TODO[tv]: choose chunk size dynamically
            //  Observations:
            //  - Up to 200Mb is OK to sort in memory
            //  - In memory merge 10-20 sequences is quick
            //  - Merging sorted files is OK fow 3-10 items
            var chunkSize = 2200000;
            var sortOptions = new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                BoundedCapacity = 2 * Environment.ProcessorCount
            };
            var batchOptions = new GroupingDataflowBlockOptions
            {
                Greedy = true
            };
            var mergeOptions = new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 4,
                MaxDegreeOfParallelism = 2
            };

            var tmpFiles = new ConcurrentBag<string>();
            var tmpFileCounter = 0;

            // Configure pipeline
            var sort = new TransformBlock<(int index, Record[] data), Record[]>(chunk =>
            {
                Log($"Sort: chunk #{chunk.index}");
                Array.Sort(chunk.data, Record.DefaultComparer);
                Log($"Sort: chunk #{chunk.index} done");
                return chunk.data;
            }, sortOptions);
            var batch = new BatchBlock<Record[]>(mergeBatchSize, batchOptions);
            var merge = new ActionBlock<Record[][]>(group =>
            {
                var i = Interlocked.Increment(ref tmpFileCounter);
                var file = string.Format(tmpFileNameFormat, i);
                Log($"Merge: batch #{tmpFileCounter}");
                RecordIO.WriteFile(file, @group.MergeSorted(Record.DefaultComparer));
                tmpFiles.Add(file);
                Log($"Merge: batch #{i} merged into '{file}'");
                Array.ForEach(@group, b => Array.Clear(b, 0, b.Length));
            }, mergeOptions);

            sort.LinkTo(batch);
            batch.LinkTo(merge);
            sort.Completion.ContinueWith(_ => batch.Complete());
            batch.Completion.ContinueWith(_ => merge.Complete());

            // Start pipeline
            var read = 0;
            foreach (var chunk in RecordIO.ChunkReadFile(inFile, chunkSize))
            {
                ++read;
                while (!sort.Post((read, chunk)))
                {
                    // Thread.Yield();
                    Thread.Sleep(100);
                }
                Log($"Parse: chunk #{read} scheduled for sorting");
            }
            sort.Complete();
            merge.Completion.Wait();

            Log("Merging tmp files");
            RecordIO.MergeFiles(tmpFiles, outFile);

            Log("Cleanup");
            foreach (var file in tmpFiles)
            {
                File.Delete(file);
            }
        }
    }
}