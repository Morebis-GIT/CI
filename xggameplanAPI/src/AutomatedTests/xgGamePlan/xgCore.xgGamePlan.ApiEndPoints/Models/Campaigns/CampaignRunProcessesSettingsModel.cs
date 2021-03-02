namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class CampaignRunProcessesSettingsModel
    {
        public string ExternalId { get; set; }
        public bool? InefficientSpotRemoval { get; set; }
        public IncludeRightSizer? IncludeRightSizer { get; set; }
    }
}
