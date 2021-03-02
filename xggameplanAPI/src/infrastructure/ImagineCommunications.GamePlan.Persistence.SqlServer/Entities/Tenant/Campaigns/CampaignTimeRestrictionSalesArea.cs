using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignTimeRestrictionSalesArea : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int CampaignTimeRestrictionId { get; set; }
        public string Name { get; set; }
    }
}
