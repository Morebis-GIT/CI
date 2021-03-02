using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;

namespace xggameplan.core.BRS
{
    public interface IBRSIndicatorManager
    {
        IEnumerable<ScenarioResult> CalculateBRSIndicators(Run run, int? brsConfigurationTemplateId = null);

        IEnumerable<ScenarioResult> CalculateBRSIndicatorsAfterRunCompleted(Run run);
    }
}
