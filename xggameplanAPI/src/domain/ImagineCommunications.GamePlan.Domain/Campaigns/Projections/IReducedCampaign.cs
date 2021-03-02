using System;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Projections
{
    public interface IReducedCampaign
    {
        int CustomId { get; }
        string ExternalId { get; }
        string Name { get; }
        string DemoGraphic { get; }
        DateTime StartDateTime { get; }
        DateTime EndDateTime { get; }
        string Product { get; }
        double RevenueBudget { get; }
        decimal TargetRatings { get; }
        decimal ActualRatings { get; }
        string CampaignGroup { get; }
        bool IsPercentage { get; }
        string Status { get; }
        string BusinessType { get; }
        CampaignDeliveryType DeliveryType { get; }
        bool IncludeOptimisation { get; }
        bool TargetZeroRatedBreaks { get; }
        bool InefficientSpotRemoval { get; }
        string ExpectedClearanceCode { get; }
        int CampaignPassPriority { get; }
        int CampaignSpotMaxRatings { get; }
    }
}
