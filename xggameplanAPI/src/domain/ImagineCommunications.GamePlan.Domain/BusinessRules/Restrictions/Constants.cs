using ImagineCommunications.GamePlan.Domain.Generic.Attributes;

namespace ImagineCommunications.GamePlan.Domain.BusinessRules.Restrictions
{
    public enum IncludeOrExcludeOrEither
    {
        /// <summary>
        /// Either. (Default)
        /// </summary>
        X = 0,

        /// <summary>
        /// Include.
        /// </summary>
        I = 1,

        /// <summary>
        /// Exclude.
        /// </summary>
        E = 2,
    }

    public enum IncludeOrExclude
    {
        /// <summary>
        /// Include
        /// </summary>
        I = 0,

        /// <summary>
        /// Exclude
        /// </summary>
        E = 1
    }



    public enum RestrictionType
    {
        [LetterValue('T')]
        Time = 1,
        [LetterValue('P')]
        Programme = 2,
        [LetterValue('Y')]
        ProgrammeCategory = 3,
        [LetterValue('I')]
        Index = 4,
        [LetterValue('C')]
        ProgrammeClassification = 5
    }

    public enum RestrictionBasis
    {
        [LetterValue('C')]
        ClearanceCode,
        [LetterValue('P')]
        Product,
        [LetterValue('H')]
        Clash
    }

    public enum RestrictionOrder
    {
        StartDate,
        EndDate,
        StartTime,
        EndTime,
        RestrictionType,
        ExternalProgRef,
        ProgrammeCategory,
        ProgrammeClassification,
        RestrictionBasis,
        ProductCode,
        ClashCode,
        ClashDescription,
        ClearanceCode,
        RestrictionDays,
        SchoolHolidayIndicator,
        PublicHolidayIndicator,
        LiveProgrammeIndicator,
        ProgrammeClassificationIndicator,
        TimeToleranceMinsBefore,
        TimeToleranceMinsAfter,
        IndexType,
        IndexThreshold,
        ClockNumber
    }
}
