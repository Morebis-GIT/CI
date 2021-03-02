using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Campaigns
{
    public class CampaignSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string CampaignExternalId { get; set; }
        public bool InefficientSpotRemoval { get; set; }
        public IncludeRightSizer IncludeRightSizer { get; set; }
        public int Priority { get; set; }
    }
}
