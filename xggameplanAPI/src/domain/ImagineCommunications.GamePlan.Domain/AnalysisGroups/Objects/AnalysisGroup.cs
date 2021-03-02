using System;

namespace ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects
{
    public class AnalysisGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public AnalysisGroupFilter Filter { get; set; }
    }
}
