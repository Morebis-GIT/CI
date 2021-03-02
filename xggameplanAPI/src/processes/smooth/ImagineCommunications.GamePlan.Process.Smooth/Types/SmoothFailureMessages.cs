namespace ImagineCommunications.GamePlan.Process.Smooth.Types
{
    /// <summary>
    /// Failure messages for Smooth grouped by type. The type indicates the
    /// overall situation.
    /// </summary>
    public enum SmoothFailureMessages
    {
        /// <summary>
        /// This is used when a value must be returned and there is no failure.
        /// Do NOT use a nullable; use THIS.
        /// </summary>
        T0_NoFailure = 0,

        // TypeID=1: Issues with spot unplaced after main pass, at least one
        // placing attempt made
        T1_InvalidBreakType = 1,
        T1_InvalidSpotTime = 2,
        T1_InsufficentRemainingDuration = 3,
        T1_ProductClash = 4,
        T1_CampaignClash = 5,
        T1_BreakPosition = 6,
        T1_RequestedPositionInBreak = 7,

        // Not actually a reason but helps identify Top/Tail spots that are split
        T1_CantAddTopAndTailToSameBreak = 8,

        // Spot unplaced, was placed in previous run
        T1_BumpedFromPreviousRun = 9,
        T1_AdvertiserClash = 10,

        // Not actually a reason but helps identify Same Break spots that are split
        T1_CantAddSameBreakToSameBreak = 11,

        // Restriction related
        T1_TimeRestrictionForProduct = 301,
        T1_TimeRestrictionForCopy = 302,
        T1_TimeRestrictionForClearanceCode = 303,
        T1_TimeRestrictionForClash = 304,
        T1_ProgrammeRestrictionForProduct = 305,
        T1_ProgrammeRestrictionForCopy = 306,
        T1_ProgrammeRestrictionForClearanceCode = 307,
        T1_ProgrammeRestrictionForClash = 308,
        T1_ProgrammeCategoryRestrictionForProduct = 309,
        T1_ProgrammeCategoryRestrictionForCopy = 310,
        T1_ProgrammeCategoryRestrictionForClearanceCode = 311,
        T1_ProgrammeCategoryRestrictionForClash = 312,
        T1_IndexRestrictionForProduct = 313,
        T1_IndexRestrictionForCopy = 314,
        T1_IndexRestrictionForClearanceCode = 315,
        T1_IndexRestrictionForClash = 316,
        T1_ProgrammeClassificationRestrictionForProduct = 317,
        T1_ProgrammeClassificationRestrictionForCopy = 318,
        T1_ProgrammeClassificationRestrictionForClearanceCode = 319,
        T1_ProgrammeClassificationRestrictionForClash = 320,
        T1_ClearanceCodeRestrictionForProduct = 321,
        T1_ClearanceCodeRestrictionForCopy = 322,
        T1_ClearanceCodeRestrictionForClearanceCode = 323,
        T1_ClearanceCodeRestrictionForClash = 324,

        // For XG 10/11
        T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingClash = 325,
        T1_SponsorshipRestrictionAppliedForCompetitorSpotBasedOnCompetingAdvertiser = 326,

        // TypeID=2: No placing attempt
        T2_NoBreakOrProgrammeData = 101,

        // TypeID=5: Issues with placed spots Spot in break #2 after run #1, now
        // in break #4 after run #2
        T3_MovedFromPreviousRun = 201,

        // TypeID=5: Issues trying to place unplaced spot by moving other spots.
        // Currently internal, sub-categorise later.
        T5_CantPlaceUnplacedSpot = 401
    }
}
