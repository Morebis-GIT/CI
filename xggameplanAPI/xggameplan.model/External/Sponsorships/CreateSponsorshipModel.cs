using System.Collections.Generic;

namespace xggameplan.Model
{
    public class CreateSponsorshipModel : SponsorshipModelBase
    {
        public IEnumerable<CreateSponsoredItemModel> SponsoredItems { get; set; }
    }
}
