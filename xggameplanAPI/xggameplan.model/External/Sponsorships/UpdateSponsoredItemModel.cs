using System.Collections.Generic;

namespace xggameplan.Model
{
    public class UpdateSponsoredItemModel : SponsoredItemModelBase
    {
        public IEnumerable<UpdateSponsorshipItemModel> SponsoredItem { get; set; }      
        public IEnumerable<UpdateClashExclusivityModel> ClashExclusivities { get; set; }
        public IEnumerable<UpdateAdvertiserExclusivityModel> AdvertiserExclusivities { get; set; }      
    }
}
