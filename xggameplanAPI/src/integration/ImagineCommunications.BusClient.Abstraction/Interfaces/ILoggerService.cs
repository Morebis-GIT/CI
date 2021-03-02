using System;
using Microsoft.Extensions.Logging;

namespace ImagineCommunications.BusClient.Abstraction.Interfaces
{
    public interface ILoggerService : ILogger
    {
        void Error(string message, Exception ex = null);

        void Info(string message, object item = null);

        void Warn(string message, object item = null);
    }
}
