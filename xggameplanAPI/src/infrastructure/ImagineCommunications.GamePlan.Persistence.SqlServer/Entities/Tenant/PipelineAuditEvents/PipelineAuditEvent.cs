using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PipelineAuditEvents
{
    public class PipelineAuditEvent : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public DateTime TimeCreated { get; set; }

        public int EventTypeId { get; set; }

        public int EventId { get; set; }

        public Guid RunId { get; set; }

        public Guid ScenarioId { get; set; }

        public string Message { get; set; }
    }
}
