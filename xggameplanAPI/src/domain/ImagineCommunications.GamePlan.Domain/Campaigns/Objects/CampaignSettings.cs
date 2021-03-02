namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class CampaignSettings
    {
        public int Id { get; set; }
        public string CampaignExternalId { get; set; }
        public bool InefficientSpotRemoval { get; set; }
        public IncludeRightSizer IncludeRightSizer { get; set; }
        public int Priority { get; set; }
    }
}
