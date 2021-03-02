using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi
{
    public class AutoBookTask : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public Guid RunId { get; set; }

        public Guid ScenarioId { get; set; }

        public int AutoBookId { get; set; }
    }
}
