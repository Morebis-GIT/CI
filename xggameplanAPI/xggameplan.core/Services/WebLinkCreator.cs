using System;

namespace xggameplan.Services
{
    /// <summary>
    /// Factory for web links
    /// </summary>
    internal class WebLinkFactory
    {
        /// <summary>
        /// Link for AutoBooks. Currently it is not possible to select a particular AutoBook.
        /// </summary>
        /// <param name="frontendUrl"></param>
        /// <returns></returns>
        public static string GetAutoBookURL(string frontendUrl, Guid autoBookId)
        {
            return string.Format(@"{0}/#/autobooks", frontendUrl);
        }

        /// <summary>
        /// Link for run details. Currently it is not possible to select a particular run.
        /// </summary>
        /// <param name="frontendUrl"></param>
        /// <param name="runId"></param>
        /// <returns></returns>
        public static string GetRunDetailsURL(string frontendUrl, Guid runId)
        {
            return string.Format(@"{0}/#/manage-runs", frontendUrl);
        }

        /// <summary>
        /// Link for Optimiser Report.
        /// </summary>
        /// <param name="frontendUrl"></param>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        public static string GetOptimiserReportURL(string frontendUrl, Guid runId, Guid scenarioId)
        {
            return string.Format(@"{0}/#/optimizer-report/{1}/{2}", frontendUrl, runId, scenarioId);
        }
    }
}
