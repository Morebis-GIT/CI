using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Scenarios
{
    public class Scenario : IUniqueIdentifierPrimaryKey, IAuditEntity
    {
        public const string SearchField = "TokenizedName";
        DateTime IAuditEntity.DateCreated
        {
            get => DateCreated.HasValue ? DateCreated.Value : default;
            set => DateCreated = value;
        }
        DateTime IAuditEntity.DateModified
        {
            get => DateModified.HasValue ? DateModified.Value : default;
            set => DateModified = value;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
        public int CustomId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime DateUserModified { get; set; }
        public bool IsAutopilot { get; set; }
        public bool? IsLibraried { get; set; }

        public ScenarioCampaignPriorityRoundCollection CampaignPriorityRounds { get; set; }
        public ICollection<ScenarioPassReference> PassReferences { get; set; }
        public ICollection<ScenarioCampaignPassPriority> CampaignPassPriorities { get; set; }
    }
}
