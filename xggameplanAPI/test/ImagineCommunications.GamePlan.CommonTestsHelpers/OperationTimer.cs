using System;
using System.Diagnostics;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.CommonTestsHelpers
{
    public sealed class OperationTimer : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly ITestOutputHelper _output;

        public OperationTimer(ITestOutputHelper output)
        {
            _output = output;
            _stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();

            _output.WriteLine("-------------------");
            _output.WriteLine($"Elapsed run time: {_stopwatch.Elapsed.TotalMilliseconds.ToString()}ms");
        }
    }
}
