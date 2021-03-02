using System.Collections.Generic;

namespace xggameplan.Model
{
    public class CreateSponsoredItemModel : SponsoredItemModelBase
    {
        public IEnumerable<CreateSponsorshipItemModel> SponsorshipItems { get; set; }       
        public IEnumerable<CreateClashExclusivityModel> ClashExclusivities { get; set; }
        public IEnumerable<CreateAdvertiserExclusivityModel> AdvertiserExclusivities { get; set; }       
    }
}



