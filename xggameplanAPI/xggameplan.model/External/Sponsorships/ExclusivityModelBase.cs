using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;

namespace xggameplan.Model
{
    public class ExclusivityModelBase
    {
        public SponsorshipRestrictionType? RestrictionType { get; set; }
        public int? RestrictionValue { get; set; }
    }
}
