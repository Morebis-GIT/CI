using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.CommonTestsHelpers
{
    public sealed class OperationTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly int _garbageCollectionCount;
        private readonly ITestOutputHelper _output;

        public OperationTimer(ITestOutputHelper output)
        {
            PrepareForOperation();

            _output = output;
            _garbageCollectionCount = GC.CollectionCount(0);

            _stopwatch = Stopwatch.StartNew();
        }

        private void PrepareForOperation()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        [SuppressMessage(
            "Performance", "HAA0101:Array allocation for params parameter",
            Justification = "Hopefully more performant than String.Concat()")
        ]
        public void Dispose()
        {
            _stopwatch.Stop();

            int garbageCollectionCount = GC.CollectionCount(0) - _garbageCollectionCount;

            _output.WriteLine("-------------------");
            _output.WriteLine("Elapsed run time: {0}", _stopwatch.Elapsed.ToString("c"));
            _output.WriteLine("Garbage Collections: {0}", garbageCollectionCount.ToString("D"));
        }
    }
}
