using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;

namespace xggameplan.Model
{
    public class CampaignPassPriorityModel
    {
        public string CampaignExternalId { get; set; }
        public CampaignWithProductFlatModel Campaign { get; set; }
        public List<PassPriorityModel> PassPriorities { get; set; }
    }
}
