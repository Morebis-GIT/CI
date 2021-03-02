using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios
{
    public class ScenarioCampaignPriorityRound : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int ScenarioCampaignPriorityRoundCollectionId { get; set; }

        public int Number { get; set; }
        public int PriorityFrom { get; set; }
        public int PriorityTo { get; set; }
    }
}
