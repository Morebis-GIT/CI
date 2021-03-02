namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    /// <summary>
    /// <para>
    /// Factors for calculating the best break to add spots to. For each break that the spot(s)
    /// could be added to  a score is calculated in order to identify the break with best score.
    /// </para>
    /// <para>
    /// Factors starting with 'Is..' are filter factors so that we can exclude breaks that don't
    /// meet the filter criteria. Filter factors calculate a 0 or 1 score, other factors create a
    /// score between 1 and N.
    /// </para>
    /// <para>
    /// Some of these factors are not currently used for NINE but they may be used with other
    /// customers later.
    /// </para>
    /// </summary>
    public enum BestBreakFactors
    {
        IsSponsoredSpotBeingPlacedWithNoBreakPosition = 100,

        IsOtherBreaksHaveSpotsForSameSponsor = 99,

        IsAnyBreaksHaveSpotsForSameSponsor = 101,

        IsNoBreaksHaveSpotsForSameSponsor = 114,

        /// <summary>
        /// Preferably first break else ascending break positions. This is typically used in
        /// conjunction with other factors in order to pick a break if they all have the same
        /// factor score.
        /// </summary>
        BreaksInAscendingPosition = 3,

        /// <summary>
        /// Preferable last break else descending break positions. This is typically used in
        /// conjunction with other factors in order to pick a break if they all have the same
        /// factor score.
        /// </summary>
        BreaksInDescendingPosition = 4,

        /// <summary>
        /// No remaining duration after spots are added, score either 0 or N
        /// </summary>
        IsFillsBreakDuration = 5,

        /// <summary>
        /// Fewest spots in break
        /// </summary>
        FewestSpotsInBreak = 6,

        /// <summary>
        /// Fewest product clashes at child level
        /// </summary>
        FewestProductClashesAtChildLevel = 7,

        /// <summary>
        /// Fewest product clashes at parent level
        /// </summary>
        FewestProductClashesAtParentLevel = 108,

        /// <summary>
        /// Fewest product clashes (both parent and child level)
        /// </summary>
        FewestProductClashes = 109,

        /// <summary>
        /// Fewest campaign clashes
        /// </summary>
        FewestCampaignClashes = 8,

        /// <summary>
        /// Fewest campaign and product clashes. For product clashes then it considers clashes at
        /// both child and parent level.
        /// </summary>
        FewestCampaignAndProductClashes = 112,

        /// <summary>
        /// Leaves default number of sec multiples for break duration (E.g. 30 secs), score
        /// either 0 or N
        /// </summary>
        IsLeavesDefaultBreakMultipleDurations = 9,

        /// <summary>
        /// <para>
        /// Leaves 15 sec multiples for break duration, score either 0 or N. This is typically
        /// used for flexibility when placing spots in to breaks that are multiples of 15
        /// (e.g. 180, 195) on the assumption that other spots will be added later to balance the
        /// break out.
        /// </para>
        /// <para>
        /// E.g. If we need to place a 15 sec spot in a 180 sec break then we assume that a
        /// 15 sec is likely to be placed later.
        /// </para>
        /// </summary>
        IsLeaves15SecBreakMultipleDurations = 107,

        /// <summary>
        /// Leaves default number of sec multiples for break duration (E.g. 30 secs) or fills
        /// break completely, score either 0 or N
        /// </summary>
        IsLeavesDefaultBreakMultipleDurationsOrFillsBreak = 106,

        /// <summary>
        /// Break efficiency (Phase 3)
        /// </summary>
        BreakEfficiency = 10,

        /// <summary>
        /// Break with largest remaining duration (so that we fill all breaks)
        /// </summary>
        LargestRemainingBreakDuration = 11,

        /// <summary>
        /// Fewest sponsor spots for same sponsor (Initially place top/tail where no sponsor
        /// spots are)
        /// </summary>
        FewestMatchingSponsorSpots = 12,

        /// <summary>
        /// No sponsor spots for same sponsor, score either 0 or N
        /// </summary>
        IsNoSponsorSpotsForSameSponsor = 13,

        /// <summary>
        /// No sponsor spots for any sponsor, score either 0 or N
        /// </summary>
        IsNoSponsorSpotsForAnySponsor = 14,

        /// <summary>
        /// Is first break, score either 0 or N
        /// </summary>
        IsFirstBreak = 15,

        /// <summary>
        /// Is first two breaks, score either 0 or N
        /// </summary>
        IsFirstTwoBreaks = 16,

        /// <summary>
        /// Is last break, score either 0 or N
        /// </summary>
        IsLastBreak = 17,

        /// <summary>
        /// Is last two breaks, score either 0 or N
        /// </summary>
        IsLastTwoBreaks = 18,

        /// <summary>
        /// Is break in first half of programme, score either 0 or N
        /// </summary>
        IsFirstHalfOfProgramme = 19,

        /// <summary>
        /// Is break in last half of programme, score either 0 or N
        /// </summary>
        IsLastHalfOfProgramme = 20,

        /// <summary>
        /// Break length is multiple of 30 secs, score either 0 or N
        /// </summary>
        IsBreakDurationIs30SecMultiples = 21,

        //IsNoProductClashes = 102,

        /// <summary>
        /// Fewested clashing requested position in break requests, only for first/last PIB
        /// </summary>
        FewestRequestedPositionInBreakRequestsFirstOrLastOnly = 104,

        /// <summary>
        /// Random break
        /// </summary>
        RandomBreak = 105,

        /// <summary>
        /// Balance spot durations, try and have a mix of spot durations so that, for example,
        /// 15 sec spots are evenly distributed
        /// </summary>
        SpotDurationBalance = 110,

        /// <summary>
        /// Same non-zero score for all breaks. This is typically used where there are filters
        /// but no default factors
        /// </summary>
        SameNonZeroScoreForAllBreaks = 113,
    }
}
