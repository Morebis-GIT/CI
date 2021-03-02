using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public class SponsoredItem
    {
        public IEnumerable<SponsorshipItem> SponsorshipItems { get; set; }
        public List<ClashExclusivity> ClashExclusivities { get; private set; }
        public List<AdvertiserExclusivity> AdvertiserExclusivities { get; private set; }
        public IEnumerable<string> Products { get; set; }
        public SponsorshipCalculationType CalculationType { get; set; }
        public SponsorshipRestrictionType? RestrictionType { get; set; }
        public int? RestrictionValue { get; set; }
        public SponsorshipApplicability? Applicability { get; set; }
    }
}
