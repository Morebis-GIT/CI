using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;

namespace xggameplan.KPIProcessing.Abstractions
{
    /// <summary>
    /// Provides functionality for resolving KPI values
    /// <seealso cref="IKPICalculationScope"/>
    /// </summary>
    public interface IKPIResolver
    {
        /// <summary>
        /// Resolves the specified KPI value by name.
        /// </summary>
        /// <param name="kpiName">KPI name.</param>
        /// <returns></returns>
        IEnumerable<KPI> Resolve(string kpiName);
    }
}
