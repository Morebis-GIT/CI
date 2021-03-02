using System.Collections.Generic;

namespace xggameplan.Model
{
    public class SponsoredItemModel : SponsoredItemModelBase
    {     
        public IEnumerable<SponsorshipItemModel> SponsorshipItems { get; set; }
        public IEnumerable<ClashExclusivityModel> ClashExclusivities { get; set; }
        public IEnumerable<AdvertiserExclusivityModel> AdvertiserExclusivities { get; set; }
    }
}
