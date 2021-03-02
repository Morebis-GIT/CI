using NodaTime;

namespace xggameplan.Model
{
    public class SmoothFailureExtendedModel
    {
        public string SmoothFailureMessages { get; set; }
        public string BreakDateTime { get; set; }
        public string BreakDate { get; set; }
        public string BreakTime { get; set; }
        public Duration SpotLength { get; set; }
        public string ExternalBreakRef { get; set; }
        public string ExternalCampaignRef { get; set; }
        public object ExternalSpotReference { get; set; }
        public string CampaignName { get; set; }
        public string CampaignGroup { get; set; }
        public string AdvertiserName { get; set; }
        public string ProductName { get; set; }
        public string ClashDescription { get; set; }
        public string IndustryCode { get; set; }
        public string ClearanceCode { get; set; }
        public string RestrictionStartDate { get; set; }
        public string RestrictionEndDate { get; set; }
        public string RestrictionStartTime { get; set; }
        public string RestrictionEndTime { get; set; }
        public string SalesArea { get; set; }
        public string RestrictionDays { get; set; }
    }
}
