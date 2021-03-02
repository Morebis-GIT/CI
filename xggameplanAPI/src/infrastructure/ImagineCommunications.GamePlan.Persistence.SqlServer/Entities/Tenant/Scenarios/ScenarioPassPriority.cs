using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios
{
    public class ScenarioPassPriority : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int ScenarioCampaignPassPriorityId { get; set; }

        public int PassId { get; set; }
        public string PassName { get; set; }
        public int Priority { get; set; }
    }
}
