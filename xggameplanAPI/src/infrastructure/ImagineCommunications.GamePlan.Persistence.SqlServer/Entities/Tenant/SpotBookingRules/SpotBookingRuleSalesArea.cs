using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SpotBookingRules
{
    public class SpotBookingRuleSalesArea : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int SpotBookingRuleId { get; set; }
        public string Name { get; set; }
    }
}
