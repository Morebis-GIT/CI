using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;

namespace xggameplan.core.BRS
{
    public interface IBRSCalculator
    {
        IEnumerable<ScenarioResult> Calculate(
            IEnumerable<BRSConfigurationForKPI> kpiConfigurations,
            IEnumerable<ScenarioResult> scenarioResults,
            IEnumerable<KPIPriority> kpiPriorities);
    }
}
