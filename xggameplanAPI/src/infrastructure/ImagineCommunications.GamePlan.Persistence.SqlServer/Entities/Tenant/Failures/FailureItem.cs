using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Failures
{
    public class FailureItem : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int FailureId { get; set; }
        public long Campaign { get; set; }
        public string CampaignName { get; set; }
        public string ExternalId { get; set; }
        public int Type { get; set; }
        public long Failures { get; set; }
        public string SalesAreaName { get; set; }
    }
}
