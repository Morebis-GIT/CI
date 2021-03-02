namespace ImagineCommunications.GamePlan.Domain.Campaigns.Queries
{
    public class CampaignFilterGroupQueryModel
    {
        public bool IsBusinessTypeFiltered { get; set; }
        public bool IsClashCodeFiltered { get; set; }
        public bool IsCampaignIdentifierFiltered { get; set; }
        public bool IsProductFiltered { get; set; }
        public bool IsAgencyFiltered { get; set; }
        public bool IsMediaSalesGroupFiltered { get; set; }
        public bool IsProductAssigneeFiltered { get; set; }
        public bool IsReportingCategoryFiltered { get; set; }
        public bool IsAdvertiserFiltered { get; set; }
    }
}
