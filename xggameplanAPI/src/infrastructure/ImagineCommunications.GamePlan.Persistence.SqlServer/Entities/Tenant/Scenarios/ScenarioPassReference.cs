using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios
{
    public class ScenarioPassReference : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int PassId { get; set; }
        public Guid ScenarioId { get; set; }
        public int Order { get; set; }

        public Pass Pass { get; set; }
    }
}
