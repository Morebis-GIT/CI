namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public abstract class SponsorshipModelBase
    {
        public string ExternalReferenceId { get; set; }
        public SponsorshipRestrictionLevel RestrictionLevel { get; set; }
    }
}
