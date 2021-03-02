using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Queries
{
    public class CampaignSearchQueryModel : BaseQueryModel
    {
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CampaignStatus Status { get; set; }
        public CampaignOrder Orderby { get; set; }
        public OrderDirection OrderDirection { get; set; }
        public bool GroupByGroupName { get; set; }

        public List<string> BusinessTypes { get; set; }
        public List<string> ClashCodes { get; set; }
        public List<string> CampaignIds { get; set; }
        public List<string> ProductIds { get; set; }
        public List<string> AgencyIds { get; set; }
        public List<string> MediaSalesGroupIds { get; set; }
        public List<string> ProductAssigneeIds { get; set; }
        public List<string> ReportingCategories { get; set; }
        public List<string> AdvertiserIds { get; set; }

        public override string ToString() => $"{Description ?? "[NoDescription]"}";
    }
}
