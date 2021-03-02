using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignTarget : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignSalesAreaTargetId { get; set; }

        public ICollection<CampaignTargetStrikeWeight> StrikeWeights { get; set; } =
            new HashSet<CampaignTargetStrikeWeight>();
    }
}
