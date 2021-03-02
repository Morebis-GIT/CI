using System.ComponentModel;

namespace ImagineCommunications.GamePlan.Domain.Campaigns
{
    public enum IncludeRightSizer
    {
        [Description("No")]
        No = 0,
        [Description("Campaign Level")]
        CampaignLevel = 1,
        [Description("Detail Level")]
        DetailLevel = 2
    }

    public enum RightSizerLevel
    {
        CampaignLevel = 0,
        DetailLevel = 1
    }

    public enum CampaignStatus
    {
        All,
        Active,
        Cancelled,
        NotApproved
    }

    public enum CampaignDeliveryType
    {
        Rating = 0,
        Spot = 1
    }

    public enum CampaignOrder
    {
        Status,
        EndDateTime,
        StartDateTime,
        Name,
        ExternalId,
        ActualRatings,
        TargetRatings,
        RevenueBudget,
        CampaignGroup,
        BusinessType,
        DeliveryType,
        IncludeRightSizer,
        InefficientSpotRemoval,
        IncludeOptimisation,
        TargetZeroRatedBreaks,
        AdvertiserName,
        AgencyName,
        ProductName,
        ClashCode,
        ClashDescription,
        Demographic,
        DiffRatings,
        TargetDelivery
    }

    public enum PassPriorityType
    {
        [Description("Exclude")]
        Exclude = 0,
        [Description("Include")]
        Include = 1000
    }


    public enum CategoryOrProgramme
    {
        /// <summary>
        /// Category
        /// </summary>
        C = 0,

        /// <summary>
        /// Programme
        /// </summary>
        P = 1
    }

    public enum DeliveryCurrency
    {
        Undefined = 0,

        /// <summary>
        /// CPT is a marketing metric used to calculate the cost to reach one thousand people or households via a given advertising outlet or medium
        /// </summary>
        CPTIndex = 2,
        RatePerMinute = 3,

        /// <summary>
        /// CPT is a marketing metric used to calculate the cost to reach one thousand people or households via a given advertising outlet or medium
        /// </summary>
        FixedCPT = 5,
        FixedRating,
        FixedSchedule,

        /// <summary>
        /// CPP is a measure of cost efficiency which enables you to compare the cost of this advertisement to other advertisements
        /// </summary>
        CPPIndex,

        /// <summary>
        /// CPP is a measure of cost efficiency which enables you to compare the cost of this advertisement to other advertisements
        /// </summary>
        FixedCPP,
        SpotPricedSchedule,
        SpotPriced,
        NonLinearOnly,
        NewMedia
    }

    public enum TopTail
    {
        Multipart,
        Either,
        Single
    }
}
