namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration
{
    public enum BestBreakFactorGroupEvaluation : byte
    {
        TotalScore = 0,
        MaxScore = 1,
        MinScore = 2,
        AvgScore = 3,
        FirstNonZeroScore = 4,
        LastNonZeroScore = 5
    }

    public enum SameBreakGroupScoreActions : byte
    {
        CheckNextGroup = 0,
        UseSingleBreakFactorIfBestScoreIsNonZero = 1
    }

    public enum BestBreakFactors
    {
        IsSponsoredSpotBeingPlacedWithNoBreakPosition = 100,
        IsOtherBreaksHaveSpotsForSameSponsor = 99,
        IsAnyBreaksHaveSpotsForSameSponsor = 101,
        IsNoBreaksHaveSpotsForSameSponsor = 114,
        BreaksInAscendingPosition = 3,
        BreaksInDescendingPosition = 4,
        IsFillsBreakDuration = 5,
        FewestSpotsInBreak = 6,
        FewestProductClashesAtChildLevel = 7,
        FewestProductClashesAtParentLevel = 108,
        FewestProductClashes = 109,
        FewestCampaignClashes = 8,
        FewestCampaignAndProductClashes = 112,
        IsLeavesDefaultBreakMultipleDurations = 9,
        IsLeaves15SecBreakMultipleDurations = 107,
        IsLeavesDefaultBreakMultipleDurationsOrFillsBreak = 106,
        BreakEfficiency = 10,
        LargestRemainingBreakDuration = 11,
        FewestMatchingSponsorSpots = 12,
        IsNoSponsorSpotsForSameSponsor = 13,
        IsNoSponsorSpotsForAnySponsor = 14,
        IsFirstBreak = 15,
        IsFirstTwoBreaks = 16,
        IsLastBreak = 17,
        IsLastTwoBreaks = 18,
        IsFirstHalfOfProgramme = 19,
        IsLastHalfOfProgramme = 20,
        IsBreakDurationIs30SecMultiples = 21,
        FewestRequestedPositionInBreakRequestsFirstOrLastOnly = 104,
        RandomBreak = 105,
        SpotDurationBalance = 110,
        SameNonZeroScoreForAllBreaks = 113,
    }

    public enum BestBreakFactorItemEvaluation : byte
    {
        TotalScore = 0,
        TotalScoreButZeroIfAnyFactorScoreIsZero = 1,
        MaxScore = 2,
        MaxScoreButZeroIfAnyFactorScoreIsZero = 3,
        MinScore = 4,
        MinScoreButZeroIfAnyFactorScoreIsZero = 5,
        AvgScore = 6,
        AvgScoreButZeroIfAnyFactorScoreIsZero = 7,
    }

    public enum ProductClashRules : byte
    {
        NoClashLimit = 0,
        NoClashes = 1,
        LimitOnExposureCount = 2
    }

    public enum SpotPositionRules : byte
    {
        Exact = 0,
        Near = 1,
        Anywhere = 2
    }

    public enum SmoothPassType
    {
        Default = 1,
        Booked = 2,
        Unplaced = 3
    }
}
