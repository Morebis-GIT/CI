using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using xggameplan.Model;

namespace xggameplan.Validations.Runs.Interfaces
{
    public interface IRunsValidator
    {
        void ValidateForSave(Run run, List<Scenario> allScenarios, List<List<Pass>> allPassesByScenario, List<SalesArea> allSalesAreas);

        void ValidateDeliveryCappingGroupIds(IEnumerable<CampaignRunProcessesSettingsModel> campaignRunProcessesSettings);
    }
}
