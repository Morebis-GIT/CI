using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Scenarios;

namespace xggameplan.Model
{
    /// <summary>
    /// Summary for runs filter
    /// </summary>
    public class FilterRunsSummaryModel
    {
        /// <summary>
        /// Authors
        /// </summary>
        public List<FilterRunsSummaryItemModel<int>> Authors = new List<FilterRunsSummaryItemModel<int>>();

        /// <summary>
        /// Scenario statuses
        /// </summary>
        public List<FilterRunsSummaryItemModel<ScenarioStatuses>> ScenarioStatuses = new List<FilterRunsSummaryItemModel<ScenarioStatuses>>();
    }
}
