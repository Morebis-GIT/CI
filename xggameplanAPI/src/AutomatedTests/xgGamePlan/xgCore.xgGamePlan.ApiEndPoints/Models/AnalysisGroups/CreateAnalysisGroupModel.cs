namespace xgCore.xgGamePlan.ApiEndPoints.Models.AnalysisGroups
{
    public class CreateAnalysisGroupModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CreateAnalysisGroupFilterModel Filter { get; set; }
    }
}
