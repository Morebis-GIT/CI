using System;
using ImagineCommunications.BusClient.Abstraction.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace ImagineCommunications.GamePlan.Intelligence.Common
{
    public class LoggerService : ILoggerService
    {
        private readonly Serilog.ILogger _logger;

        public LoggerService(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public void Error(string message, Exception ex = null)
        {
            _logger.Write(LogEventLevel.Error, "Message: {1},{0} Exception: {2}", Environment.NewLine, message, ex);
        }

        public void Info(string message, object item = null)
        {
            _logger.Write(LogEventLevel.Information, item != null ? "Message: {1},{0} Data: {2}" : "Message: {1}", Environment.NewLine, message, item);
        }

        public void Warn(string message, object item = null)
        {
            _logger.Write(LogEventLevel.Warning, item != null ? "Message: {1},{0} Data: {2}" : "Message: {1}", Environment.NewLine, message, item);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(ConvertToSerilogLevel(logLevel));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Write(ConvertToSerilogLevel(logLevel), formatter.Invoke(state, exception), exception);
        }

        private LogEventLevel ConvertToSerilogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return LogEventLevel.Verbose;

                case LogLevel.Debug:
                    return LogEventLevel.Debug;

                case LogLevel.Information:
                    return LogEventLevel.Information;

                case LogLevel.Warning:
                    return LogEventLevel.Warning;

                case LogLevel.Error:
                    return LogEventLevel.Error;

                case LogLevel.Critical:
                    return LogEventLevel.Fatal;

                case LogLevel.None:
                    return LogEventLevel.Verbose;

                default:
                    return LogEventLevel.Verbose;
            }
        }
    }
}
