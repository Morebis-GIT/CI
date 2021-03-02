using System.Collections.Generic;

namespace xggameplan.Model
{
    public class CampaignFilterGroupModel
    {
        public IEnumerable<string> BusinessTypes { get; set; } = new List<string>();

        public IEnumerable<CampaignFilterGroupDataModel> ClashCodes { get; set; } = new List<CampaignFilterGroupDataModel>();

        public IEnumerable<CampaignFilterGroupDataModel> Campaigns { get; set; } = new List<CampaignFilterGroupDataModel>();

        public IEnumerable<CampaignFilterGroupDataModel> Products { get; set; } = new List<CampaignFilterGroupDataModel>();

        public IEnumerable<CampaignFilterGroupDataModel> Agencies { get; set; } = new List<CampaignFilterGroupDataModel>();

        public IEnumerable<CampaignFilterGroupDataModel> MediaSalesGroups { get; set; } = new List<CampaignFilterGroupDataModel>();

        public IEnumerable<CampaignFilterGroupDataModel> ProductAssignees { get; set; } = new List<CampaignFilterGroupDataModel>();

        public IEnumerable<string> ReportingCategories { get; set; } = new List<string>();

        public IEnumerable<CampaignFilterGroupDataModel> Advertisers { get; set; } = new List<CampaignFilterGroupDataModel>();
    }
}
