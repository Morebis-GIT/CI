using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class Rule : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int RuleId { get; set; }
        public int RuleTypeId { get; set; }
        public string InternalType { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public CampaignDeliveryType? CampaignType { get; set; }
    }
}
