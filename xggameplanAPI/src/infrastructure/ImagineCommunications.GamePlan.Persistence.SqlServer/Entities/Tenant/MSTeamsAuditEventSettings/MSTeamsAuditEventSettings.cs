using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.MSTeamsAuditEventSettings
{
    public class MSTeamsAuditEventSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public int EventTypeId { get; set; }

        public string MessageCreatorId { get; set; }

        public MSTeamsPostMessageSettings PostMessageSettings { get; set; }
    }
}
