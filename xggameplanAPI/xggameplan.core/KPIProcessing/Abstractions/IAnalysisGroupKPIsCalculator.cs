using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;

namespace xggameplan.KPIProcessing.Abstractions
{
    /// <summary>
    /// Functionality for calculating run analysis group KPIs.
    /// </summary>
    public interface IAnalysisGroupKPIsCalculator
    {
        /// <summary>
        /// Calculates analysis group KPIs for run.
        /// </summary>
        /// <param name="runId">Run identifier.</param>
        /// <returns></returns>
        List<AnalysisGroupTargetMetric> CalculateAnalysisGroupKPIs(Guid runId);
    }
}
