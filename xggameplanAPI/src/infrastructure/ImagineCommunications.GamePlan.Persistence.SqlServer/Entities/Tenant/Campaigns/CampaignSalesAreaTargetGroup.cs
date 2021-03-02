using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignSalesAreaTargetGroup : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignSalesAreaTargetId { get; set; }
        public string GroupName { get; set; }

        public ICollection<CampaignSalesAreaTargetGroupSalesArea> SalesAreas { get; set; } =
            new HashSet<CampaignSalesAreaTargetGroupSalesArea>();
    }
}
