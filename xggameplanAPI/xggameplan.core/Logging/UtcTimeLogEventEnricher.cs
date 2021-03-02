using System;
using Serilog.Core;
using Serilog.Events;

namespace xggameplan.core.Logging
{
    public class UtcTimeLogEventEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("UtcTime",
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff")));
        }
    }
}
