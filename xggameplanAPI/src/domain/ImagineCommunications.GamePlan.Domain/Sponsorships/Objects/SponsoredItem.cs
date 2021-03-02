using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;

namespace ImagineCommunications.GamePlan.Domain.Sponsorships.Objects
{
    public class SponsoredItem
    {
        public IEnumerable<SponsorshipItem> SponsorshipItems { get; set; }
        public List<ClashExclusivity> ClashExclusivities { get; set; }
        public List<AdvertiserExclusivity> AdvertiserExclusivities { get; set; }
        public IEnumerable<string> Products { get; set; }
        public SponsorshipCalculationType CalculationType { get; set; }
        public SponsorshipRestrictionType? RestrictionType { get; set; }
        public int? RestrictionValue { get; set; }
        public SponsorshipApplicability? Applicability { get; set; }

        public SponsoredItem Clone() => (SponsoredItem)MemberwiseClone();
    }
}
