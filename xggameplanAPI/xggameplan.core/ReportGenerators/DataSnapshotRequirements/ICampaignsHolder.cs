using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns.Projections;

namespace xggameplan.core.ReportGenerators.DataSnapshotRequirements
{
    public interface ICampaignsHolder
    {
        IEnumerable<IReducedCampaign> Campaigns { get; }
    }
}
