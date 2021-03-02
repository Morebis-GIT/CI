using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public class SponsorshipModel : BaseModel
    {
        public string ExternalReferenceId { get; set; }
        public SponsorshipRestrictionLevel RestrictionLevel { get; set; }
        public IEnumerable<SponsoredItemModel> SponsoredItems { get; set; }
    }
}
