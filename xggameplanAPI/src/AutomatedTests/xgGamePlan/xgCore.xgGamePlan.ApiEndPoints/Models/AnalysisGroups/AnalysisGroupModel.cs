using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.AnalysisGroups
{
    public class AnalysisGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public AuthorModel CreatedBy { get; set; }
        public DateTime DateModified { get; set; }
        public AuthorModel ModifiedBy { get; set; }
        public AnalysisGroupFilterModel Filter { get; set; }
    }
}
