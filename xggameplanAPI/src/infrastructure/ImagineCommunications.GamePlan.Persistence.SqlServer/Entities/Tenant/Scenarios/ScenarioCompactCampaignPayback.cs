using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios
{
    public class ScenarioCompactCampaignPayback : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int ScenarioCompactCampaignId { get; set; }
        public string Name { get; set; }
        public double Amount { get; set; }
    }
}
