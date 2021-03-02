using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios
{
    public class ScenarioCampaignPriorityRoundCollection : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid ScenarioId { get; set; }

        public bool ContainsInclusionRound { get; set; }
        public ICollection<ScenarioCampaignPriorityRound> Rounds { get; set; }
    }
}
