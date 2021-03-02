using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace ImagineCommunications.GamePlan.Core.Tests
{
    /// <summary>
    /// Stub class for verifying messages posted through the ILogger interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StubLogger<T> : ILogger<T>
    {
        private readonly ITestOutputHelper _output;

        public StubLogger(ITestOutputHelper output) => _output = output;

        /// <summary>
        /// A record of the messages written to the ILogger.
        /// </summary>
        public List<string> Messages { get; } = new List<string>();

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            Messages.Add(state.ToString());
            _output.WriteLine(state.ToString());
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => null;
    }
}
