using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using xggameplan.Model;

namespace xggameplan.core.Interfaces
{
    public interface ICampaignFlattener
    {
        List<CampaignFlattenedModel> Flatten(IReadOnlyCollection<Campaign> campaigns);
    }
}
