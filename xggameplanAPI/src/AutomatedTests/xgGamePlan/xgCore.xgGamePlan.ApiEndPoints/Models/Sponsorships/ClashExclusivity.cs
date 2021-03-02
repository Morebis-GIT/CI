namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public class ClashExclusivity
    {
        public string ClashExternalRef { get; set; }
        public SponsorshipRestrictionType? RestrictionType { get; set; }
        public int? RestrictionValue { get; set; }
    }
}
