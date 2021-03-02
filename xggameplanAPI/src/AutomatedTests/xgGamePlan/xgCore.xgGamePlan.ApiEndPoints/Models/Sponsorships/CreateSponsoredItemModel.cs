using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public class CreateSponsoredItemModel : SponsoredItemModelBase
    {
        public IEnumerable<CreateSponsorshipItemModel> SponsorshipItems { get; set; }
        public IEnumerable<CreateClashExclusivityModel> ClashExclusivities { get; set; }
        public IEnumerable<CreateAdvertiserExclusivityModel> AdvertiserExclusivities { get; set; }
    }
}
