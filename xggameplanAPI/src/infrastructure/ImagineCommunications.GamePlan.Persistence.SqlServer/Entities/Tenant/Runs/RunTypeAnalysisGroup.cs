using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AnalysisGroups;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs
{
    public class RunTypeAnalysisGroup : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int RunTypeId { get; set; }
        public int AnalysisGroupId { get; set; }
        public string KPI { get; set; }

        public RunType RunType { get; set; }
        public AnalysisGroup AnalysisGroup { get; set; }
    }
}
