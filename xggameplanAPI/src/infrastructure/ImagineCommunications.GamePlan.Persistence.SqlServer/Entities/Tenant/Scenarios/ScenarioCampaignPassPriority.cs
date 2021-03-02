using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios
{
    public class ScenarioCampaignPassPriority : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid ScenarioId { get; set; }

        public ScenarioCompactCampaign Campaign { get; set; }
        public IEnumerable<ScenarioPassPriority> PassPriorities { get; set; }
    }
}
