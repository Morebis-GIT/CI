using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignSalesAreaTargetGroupSalesArea : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignSalesAreaTargetGroupId { get; set; }
        public string Name { get; set; }
    }
}
