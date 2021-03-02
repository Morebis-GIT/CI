using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.AnalysisGroups.Objects
{
    public class AnalysisGroupFilter
    {
        public HashSet<string> AdvertiserExternalIds { get; set; } = new HashSet<string>();
        public HashSet<string> AgencyExternalIds { get; set; } = new HashSet<string>();
        public HashSet<string> AgencyGroupCodes { get; set; } = new HashSet<string>();
        public HashSet<string> BusinessTypes { get; set; } = new HashSet<string>();
        public HashSet<string> CampaignExternalIds { get; set; } = new HashSet<string>();
        public HashSet<string> ClashExternalRefs { get; set; } = new HashSet<string>();
        public HashSet<string> ProductExternalIds { get; set; } = new HashSet<string>();
        public HashSet<string> ReportingCategories { get; set; } = new HashSet<string>();
        public HashSet<int> SalesExecExternalIds { get; set; } = new HashSet<int>();

        public static AnalysisGroupFilter BuildFrom(IEnumerable<AnalysisGroup> analysisGroups)
        {
            var result = new AnalysisGroupFilter();

            foreach (var analysisGroup in analysisGroups)
            {
                result.AdvertiserExternalIds.UnionWith(analysisGroup.Filter.AdvertiserExternalIds);
                result.AgencyExternalIds.UnionWith(analysisGroup.Filter.AgencyExternalIds);
                result.AgencyGroupCodes.UnionWith(analysisGroup.Filter.AgencyGroupCodes);
                result.BusinessTypes.UnionWith(analysisGroup.Filter.BusinessTypes);
                result.CampaignExternalIds.UnionWith(analysisGroup.Filter.CampaignExternalIds);
                result.ClashExternalRefs.UnionWith(analysisGroup.Filter.ClashExternalRefs);
                result.ProductExternalIds.UnionWith(analysisGroup.Filter.ProductExternalIds);
                result.ReportingCategories.UnionWith(analysisGroup.Filter.ReportingCategories);
                result.SalesExecExternalIds.UnionWith(analysisGroup.Filter.SalesExecExternalIds);
            }

            return result;
        }
    }
}
