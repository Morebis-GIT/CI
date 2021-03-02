using System;

namespace ImagineCommunications.GamePlan.Domain.Runs.Objects
{
    public class AnalysisGroupTarget
    {
        public Guid Id { get; set; }
        public int AnalysisGroupId { get; set; }
        public string KPI { get; set; }
        public double Target { get; set; }
        public int SortIndex { get; set; }
    }
}
