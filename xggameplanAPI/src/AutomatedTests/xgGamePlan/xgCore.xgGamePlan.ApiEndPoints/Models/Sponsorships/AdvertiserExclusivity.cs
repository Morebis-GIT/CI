namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public class AdvertiserExclusivity
    {
        public string AdvertiserIdentifier { get; set; }
        public SponsorshipRestrictionType? RestrictionType { get; set; }
        public int? RestrictionValue { get; set; }
    }
}
