using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.BestBreakFactorEntities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration
{
    public class SmoothConfiguration
    {
        public int Id { get; set; }

        public string Version { get; set; }

        public bool RestrictionCheckEnabled { get; set; }

        public bool ClashExceptionCheckEnabled { get; set; }

        public ICollection<string> ExternalCampaignRefsToExclude { get; set; }

        public bool RecommendationsForExcludedCampaigns { get; set; }

        public bool SmoothFailuresForExcludedCampaigns { get; set; }

        public ICollection<SmoothPass> Passes { get; set; }

        public ICollection<SmoothPassIterationRecord> IterationRecords { get; set; }

        public ICollection<BestBreakFactorGroupRecord> BestBreakFactorGroupRecords { get; set; }

        public SmoothDiagnosticConfiguration DiagnosticConfiguration { get; set; }
    }
}
