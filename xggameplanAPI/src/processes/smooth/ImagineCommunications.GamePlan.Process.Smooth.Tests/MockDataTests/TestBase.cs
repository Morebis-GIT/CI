using System;
using System.Diagnostics;
using AutoFixture;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Process.Smooth.Tests
{
    public class TestBase
    {
        private readonly ITestOutputHelper _output;

        protected TestBase(ITestOutputHelper output) => _output = output;

        protected Fixture Fixture { get; } = new SafeFixture();

        /// <summary>Execute the method under test and track how long it took to
        /// run.</summary>
        /// <param name="methodToTest">The method to test.</param>
        /// <returns>The method results to examine.</returns>
        protected TResult Act<TResult>(
            Func<TResult> methodToTest)
        {
            var stopwatch = Stopwatch.StartNew();

            var result = methodToTest();

            stopwatch.Stop();
            _output.WriteLine(
                $"Executed in {stopwatch.ElapsedMilliseconds.ToString()}ms"
                );

            return result;
        }
    }
}
