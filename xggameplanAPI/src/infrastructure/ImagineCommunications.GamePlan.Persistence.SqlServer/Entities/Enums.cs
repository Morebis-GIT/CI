using System.ComponentModel;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities
{
    public enum IncludeOrExclude
    {
        [Description("I")]
        Include,
        [Description("E")]
        Exclude
    }

    public enum CategoryOrProgramme
    {
        [Description("C")]
        Category,
        [Description("P")]
        Programme
    }

    public enum CampaignStatus
    {
        [Description("A")]
        Active = 1,
        [Description("C")]
        Cancelled,
        [Description("N")]
        NotApproved
    }

    public enum IncludeOrExcludeOrEither
    {
        [Description("X")]
        Either = 0,
        [Description("I")]
        Include = 1,
        [Description("E")]
        Exclude = 2
    }

    public enum RestrictionType
    {
        Time = 1,
        Programme = 2,
        ProgrammeCategory = 3,
        Index = 4,
        ProgrammeClassification = 5
    }

    public enum RestrictionBasis
    {
        ClearanceCode = 0,
        Product = 1,
        Clash = 2
    }

    public enum EfficiencyCalculationPeriod
    {
        RunPeriod = 0,
        NumberOfWeeks = 1
    }

    public enum SalesAreaPriorityType
    {
        Exclude = 0,
        Priority1 = 1,
        Priority2 = 2,
        Priority3 = 3
    }

    public enum RightSizerLevel
    {
        CampaignLevel = 0,
        DetailLevel = 1
    }

    public enum RunStatus
    {
        InProgress,
        Complete,
        Errors,
        NotStarted
    }

    public enum ScenarioStatus : short
    {
        Pending = 0,
        Scheduled,
        Starting,
        InProgress,
        GettingResults,
        CompletedSuccess,
        CompletedError,
        Deleted,
        Smoothing
    }

    public enum ForceOverUnder
    {
        None = 0,
        Under = 1,
        Over = 2
    }

    public enum CampaignDeliveryType
    {
        Rating = 0,
        Spot = 1
    }

    public enum SponsorshipCalculationType
    {
        None = 0,
        Exclusive = 1,
        Percentage = 2,
        Flat = 3,
    }

    public enum SponsorshipRestrictionLevel
    {
        Programme = 0,
        TimeBand = 1
    }

    public enum SponsorshipRestrictionType
    {
        SpotCount = 0,
        SpotDuration = 1
    }

    public enum SponsorshipApplicability
    {
        AllCompetitors = 0,
        EachCompetitor = 1
    }

    public enum PassRuleType
    {
        Undefined = 0,
        General = 1,
        Rule = 2,
        Tolerance = 3,
        Weighting = 4
    }

    public enum CampaignDeliveryCurrency
    {
        Undefined = 0,

        /// <summary>
        /// CPT is a marketing metric used to calculate the cost to reach one
        /// thousand people or households via a given advertising outlet or medium
        /// </summary>
        CPTIndex = 2,
        RatePerMinute = 3,

        /// <summary>
        /// CPT is a marketing metric used to calculate the cost to reach one
        /// thousand people or households via a given advertising outlet or medium
        /// </summary>
        FixedCPT = 5,
        FixedRating,
        FixedSchedule,

        /// <summary>
        /// CPP is a measure of cost efficiency which enables you to compare the
        /// cost of this advertisement to other advertisements
        /// </summary>
        CPPIndex,

        /// <summary>
        /// CPP is a measure of cost efficiency which enables you to compare the
        /// cost of this advertisement to other advertisements
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
