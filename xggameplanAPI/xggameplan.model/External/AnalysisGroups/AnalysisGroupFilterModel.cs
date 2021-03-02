using System.Collections.Generic;

namespace xggameplan.Model
{
    public class AnalysisGroupFilterModel
    {
        public List<AnalysisGroupFilterItem<string>> Advertisers { get; set; }
        public List<AnalysisGroupFilterItem<string>> Agencies { get; set; }
        public List<AnalysisGroupFilterItem<string>> MediaGroup { get; set; }
        public List<AnalysisGroupFilterItem<string>> BusinessTypes { get; set; }
        public List<AnalysisGroupFilterItem<string>> Campaigns { get; set; }
        public List<AnalysisGroupFilterItem<string>> ClashCodes { get; set; }
        public List<AnalysisGroupFilterItem<string>> Products { get; set; }
        public List<AnalysisGroupFilterItem<string>> ReportingCategories { get; set; }
        public List<AnalysisGroupFilterItem<int>> ProductAssigneeName { get; set; }
    }
}
