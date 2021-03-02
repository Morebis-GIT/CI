using System;

namespace xggameplan.Model
{
    public class AnalysisGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public UserReducedModel CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public UserReducedModel ModifiedBy { get; set; }
        public AnalysisGroupFilterModel Filter { get; set; }
    }
}
