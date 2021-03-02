using System;

namespace xggameplan.KPIProcessing.Abstractions
{
    /// <summary>
    /// Provides functionality to create KPI execution context.
    /// </summary>
    public interface IKPICalculationScopeFactory
    {
        /// <summary>
        /// Creates the KPI calculation scope for scenario.
        /// </summary>
        /// <param name="runId">The run identifier.</param>
        /// <param name="scenarioId">The run scenario identifier.</param>
        /// <returns></returns>
        IKPICalculationScope CreateCalculationScope(Guid runId, Guid scenarioId);
    }
}
