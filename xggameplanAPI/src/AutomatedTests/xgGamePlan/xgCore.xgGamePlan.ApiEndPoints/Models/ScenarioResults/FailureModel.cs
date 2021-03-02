namespace xgCore.xgGamePlan.ApiEndPoints.Models.ScenarioResults
{
    public class FailureModel
    {
        public long Campaign { get; set; }
        public string CampaignName { get; set; }
        public string ExternalId { get; set; }
        public int Type { get; set; }
        public string SalesAreaName { get; set; }
        public string SalesAreaShortName { get; set; }
        public long Failures { get; set; }
    }
}
