using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public class Sponsorship : EntityBase
    {
        public string ExternalReferenceId { get; set; }
        public SponsorshipRestrictionLevel RestrictionLevel { get; set; }
        public List<SponsoredItem> SponsoredItems { get; private set; }
    }
}
