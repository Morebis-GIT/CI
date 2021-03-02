using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Sponsorships.Enums;

namespace xggameplan.Model
{
    public class SponsorshipModel : BaseModel
    {
        public string ExternalReferenceId { get; set; }
        public SponsorshipRestrictionLevel RestrictionLevel { get; set; }
        public IEnumerable<SponsoredItemModel> SponsoredItems { get; set; }       
    }
}
