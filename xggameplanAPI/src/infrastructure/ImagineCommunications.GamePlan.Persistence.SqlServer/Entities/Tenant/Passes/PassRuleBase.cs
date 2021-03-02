using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public abstract class PassRuleBase : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int PassId { get; set; }
        public int RuleId { get; set; }
        public string InternalType { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }
}
