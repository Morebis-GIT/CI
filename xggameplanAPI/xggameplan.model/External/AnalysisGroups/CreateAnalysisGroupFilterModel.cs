using System.Collections.Generic;

namespace xggameplan.Model
{
    public class CreateAnalysisGroupFilterModel
    {
        public HashSet<string> AdvertiserExternalIds { get; set; }
        public HashSet<string> AgencyExternalIds { get; set; }
        public HashSet<string> AgencyGroupCodes { get; set; }
        public HashSet<string> BusinessTypes { get; set; }
        public HashSet<string> CampaignExternalIds { get; set; }
        public HashSet<string> ClashExternalRefs { get; set; }
        public HashSet<string> ProductExternalIds { get; set; }
        public HashSet<string> ReportingCategories { get; set; }
        public HashSet<int> SalesExecExternalIds { get; set; }
    }
}
