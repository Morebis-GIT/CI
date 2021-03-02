using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using xggameplan.AuditEvents;
using xggameplan.core.FeatureManagement.Interfaces;

namespace xggameplan.KPIProcessing.Abstractions
{
    /// <summary>
    /// Manager class for all scenario KPIs calculators to resolve and calculate all needed KPIs.
    /// </summary>
    public interface IKPICalculationManager
    {
        /// <summary>
        /// Sets the current logger.
        /// </summary>
        /// <param name="audit">The audit.</param>
        /// <returns></returns>
        IKPICalculationManager SetAudit(IAuditEventRepository audit);

        /// <summary>
        /// Calculate all KPIs needed by resolving them by names provided and running calculations,
        /// all exceptions and problems in the process are logged (given that audit was provided via constructor)
        /// but never exposed to reflect the exact behaviour of scenario KPIs calculation process
        /// from before refactoring done in scope of XGGT-14213
        /// </summary>
        /// <param name="kpiNamesToCalculate">KPI names to calculate.</param>
        /// <param name="runId">Run identifier.</param>
        /// <param name="scenarioId">Scenario identifier.</param>
        /// <returns>KPIs collection</returns>
        List<KPI> CalculateKPIs(HashSet<string> kpiNamesToCalculate, Guid runId, Guid scenarioId);

        /// <summary>
        /// Calculates KPIs for the default set of available KPI names.
        /// Uses <see cref="IFeatureManager"/> to determine which KPIs to include.
        /// </summary>
        /// <param name="runId">Run identifier.</param>
        /// <param name="scenarioId">Scenario identifier.</param>
        /// <returns>KPIs collection</returns>
        List<KPI> CalculateKPIs(Guid runId, Guid scenarioId);

        /// <summary>
        /// Calculates the analysis group KPIs.
        /// </summary>
        /// <param name="runId">Run identifier.</param>
        /// <param name="scenarioId">Scenario identifier.</param>
        /// <returns></returns>
        List<AnalysisGroupTargetMetric> CalculateAnalysisGroupKPIs(Guid runId);
    }
}
