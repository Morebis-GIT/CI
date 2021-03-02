using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using xggameplan.Model;

namespace xggameplan.core.ReportGenerators.Interfaces
{
    /// <summary>
    /// Scenario campaign result report creator.
    /// </summary>
    public interface IScenarioCampaignResultReportCreator
        : IReportCreator<ScenarioCampaignExtendedResultItem, ScenarioCampaignResultModel>
    {
        /// <summary>
        /// Enable or disable additional performance KPI columns.
        /// </summary>
        public void EnablePerformanceKPIColumns(bool areEnabled);

        /// <summary>
        /// Enable campaign level data report instead of daypart level.
        /// </summary>
        public void EnableCampaignLevel(bool areEnabled);

        /// <summary>
        /// Generates the report.
        /// </summary>
        /// <param name="getSourceAction">Delegate, used for getting source data</param>
        /// <returns></returns>
        Stream GenerateReport(Func<IReadOnlyCollection<ScenarioCampaignExtendedResultItem>> getSourceAction);
    }
}
