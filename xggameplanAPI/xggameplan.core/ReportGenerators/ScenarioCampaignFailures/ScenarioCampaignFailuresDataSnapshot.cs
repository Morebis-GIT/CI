using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns.Projections;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using xggameplan.core.ReportGenerators.DataSnapshotRequirements;

namespace xggameplan.core.ReportGenerators.ScenarioCampaignFailures
{
    public class ScenarioCampaignFailuresDataSnapshot : IFaultTypesHolder, ICampaignsHolder
    {
        public IEnumerable<IReducedCampaign> Campaigns { get; set; }
        public IEnumerable<FaultType> FaultTypes { get; set; }
    }
}
