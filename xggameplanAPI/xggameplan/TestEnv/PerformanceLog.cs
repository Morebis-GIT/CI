using System;
using System.Globalization;
using System.IO;
using System.Web.Hosting;
using ImagineCommunications.GamePlan.Domain.Generic.Types;

namespace xggameplan.TestEnv
{
    //It is used by ILogger<T> to register and resolve performance tests logger
    public sealed class PerformanceLog
    {
        private static string GetLogDateTimeFormat(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static string[] FindLogFiles(DateTime? dateTime = null)
        {
            var datePattern = dateTime.HasValue ? GetLogDateTimeFormat(dateTime.Value) : "*";
            return Directory.GetFiles(HostingEnvironment.MapPath("/Logs"),
                $"perf-test-*-{datePattern}.log");
        }

        public static string GetLogFileNameTemplate(TenantIdentifier tenantIdentifier)
        {
            if (tenantIdentifier == null)
            {
                throw new ArgumentNullException(nameof(tenantIdentifier));
            }
            return Path.Combine(HostingEnvironment.MapPath("/Logs"), $"perf-test-{tenantIdentifier.Name}-.log");
        }
    }
}
