using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.AnalysisGroups
{
    public class AnalysisGroupFilterModel
    {
        public List<AnalysisGroupFilterItem> Advertisers { get; set; }
        public List<AnalysisGroupFilterItem> Agencies { get; set; }
        public List<AnalysisGroupFilterItem> AgencyGroups { get; set; }
        public List<AnalysisGroupFilterItem> BusinessTypes { get; set; }
        public List<AnalysisGroupFilterItem> Campaigns { get; set; }
        public List<AnalysisGroupFilterItem> ClashCodes { get; set; }
        public List<AnalysisGroupFilterItem> Products { get; set; }
        public List<AnalysisGroupFilterItem> ReportingCategories { get; set; }
        public List<AnalysisGroupFilterItem> SalesExecs { get; set; }
    }
}
