using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public class CreateSponsorshipModel : SponsorshipModelBase
    {
        public IEnumerable<CreateSponsoredItemModel> SponsoredItems { get; set; }
    }
}
