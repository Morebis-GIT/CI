namespace xgCore.xgGamePlan.ApiEndPoints.Models.Clashes
{
    public class ClashSearchQueryModel
    {
        public string NameOrRef { get; set; }
        public int? Top { get; set; }
        public int? Skip { get; set; }
    }
}
