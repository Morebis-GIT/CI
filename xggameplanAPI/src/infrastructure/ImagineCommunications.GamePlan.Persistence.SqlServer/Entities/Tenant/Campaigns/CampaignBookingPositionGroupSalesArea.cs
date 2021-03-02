using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignBookingPositionGroupSalesArea : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignBookingPositionGroupId { get; set; }
        public string Name { get; set; }
    }
}
