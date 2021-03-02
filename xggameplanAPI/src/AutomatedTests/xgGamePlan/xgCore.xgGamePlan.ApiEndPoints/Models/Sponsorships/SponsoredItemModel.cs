using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Sponsorships
{
    public class SponsoredItemModel : SponsoredItemModelBase
    {
        public IEnumerable<SponsorshipItemModel> SponsorshipItems { get; set; }
        public IEnumerable<ClashExclusivityModel> ClashExclusivities { get; set; }
        public IEnumerable<AdvertiserExclusivityModel> AdvertiserExclusivities { get; set; }
    }
}
