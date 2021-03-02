using System.Collections.Generic;

namespace xggameplan.AutoBooks.Abstractions
{
    /// <summary>
    /// Interface for Autobook Provider API - Logs Endpoint
    /// </summary>
    public interface IProviderLogsAPI
    {
        /// <summary>
        /// Gets a List of Logs from the Autobook Provider API
        /// </summary>
        List<ProviderLog> GetLogs(string logDate);
    }
}
