using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;

namespace xggameplan.KPIProcessing.Abstractions
{
    /// <summary>
    /// Provides functionality for caching already calculated KPI values.
    /// </summary>
    /// <remarks>
    /// Is valid in the scope of <see cref="IKPICalculationScope"/>.
    /// </remarks>
    public interface IKPICache
    {
        /// <summary>
        /// Gets cached KPI value.
        /// </summary>
        /// <param name="name">KPI name</param>
        /// <returns></returns>
        KPI GetKPI(string name);

        /// <summary>
        /// Adds KPIs to the cache if not present.
        /// </summary>
        /// <param name="kpiCollection">KPIs kpiCollection.</param>
        /// <returns></returns>
        void AddKPIs(IEnumerable<KPI> kpiCollection);
    }
}
