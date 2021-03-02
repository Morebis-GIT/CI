using System;
using System.Web;
using Serilog.Core;
using Serilog.Events;

namespace xggameplan.Logging
{
    public class PerformanceLogCorrelationIdLogEventEnricher : ILogEventEnricher
    {
        private readonly string _header;
        private const string HttpContextItemName = "__perf_test_correlation_id__";

        public PerformanceLogCorrelationIdLogEventEnricher(string header)
        {
            _header = header;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Items[HttpContextItemName] == null)
                {
                    var correlationId = HttpContext.Current.Request.Headers[_header] ?? Guid.NewGuid().ToString();
                    HttpContext.Current.Items[HttpContextItemName] = correlationId;
                }

                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("CorrelationId",
                    HttpContext.Current.Items[HttpContextItemName].ToString()));
            }
        }
    }
}
