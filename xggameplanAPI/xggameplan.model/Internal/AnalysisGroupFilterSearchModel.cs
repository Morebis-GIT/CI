using System.Collections.Generic;

namespace xggameplan.Model
{
    public class AnalysisGroupFilterSearchModel
    {
        public Dictionary<string, string> Advertisers { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Agencies { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> AgencyGroups { get; set; } = new Dictionary<string, string>();
        public HashSet<string> BusinessTypes { get; set; } = new HashSet<string>();
        public Dictionary<string, string> Campaigns { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> ClashCodes { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Products { get; set; } = new Dictionary<string, string>();
        public HashSet<string> ReportingCategories { get; set; } = new HashSet<string>();
        public Dictionary<int, string> SalesExecs { get; set; } = new Dictionary<int, string>();
    }
}
