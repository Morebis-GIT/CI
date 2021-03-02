using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs
{
    public class RunAnalysisGroupTarget : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid RunId { get; set; }
        public Guid AnalysisGroupTargetId { get; set; }
        public int AnalysisGroupId { get; set; }
        public string KPI { get; set; }
        public double Target { get; set; }
        public int SortIndex { get; set; }
    }
}
