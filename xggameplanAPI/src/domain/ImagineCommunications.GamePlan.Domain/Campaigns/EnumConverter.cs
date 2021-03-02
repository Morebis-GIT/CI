namespace ImagineCommunications.GamePlan.Domain.Campaigns
{
    public static class EnumConverter
    {
        public static string ConvertToSearchKeys(this CampaignStatus status)
        {
            switch (status)
            {
                case CampaignStatus.All:
                    return "A C N";
                case CampaignStatus.Active:
                    return "A";
                case CampaignStatus.Cancelled:
                    return "C";
                case CampaignStatus.NotApproved:
                    return "N";
                default:
                    return null;
            }
        }
    }
}
