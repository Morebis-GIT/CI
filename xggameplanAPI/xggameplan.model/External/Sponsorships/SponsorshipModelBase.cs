using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;

namespace xggameplan.Model
{
    public class SponsorshipModelBase
    {
        public string ExternalReferenceId { get; set; }
        public SponsorshipRestrictionLevel RestrictionLevel { get; set; }      
    }
}
