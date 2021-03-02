using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class CampaignPassPriority
    {
        public CompactCampaign Campaign { get; set; }
        public List<PassPriority> PassPriorities { get; set; }
    }
}
