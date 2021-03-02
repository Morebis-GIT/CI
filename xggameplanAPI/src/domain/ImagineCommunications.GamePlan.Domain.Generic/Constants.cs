namespace ImagineCommunications.GamePlan.Domain.Generic
{
    /// <summary>
    /// Defines order direction.
    /// </summary>
    public enum OrderDirection
    {
        /// <summary>
        /// Ascending (default).
        /// </summary>
        Asc,

        /// <summary>
        /// Descending.
        /// </summary>
        Desc
    }

    public enum Operator
    {
        Equal = 0,
        NotEqual = 1,
        GreaterThan = 2,
        GreaterThanEqual = 3,
        LessThan = 4,
        LessThanEqual = 5,
        DataTypeCheck = 6
    }

    public enum OrderBy
    {
        Title,
        Date,
    }

    public enum Dayparts
    {
        OffPeak,
        Peak,
        MidnightToDawn
    }
}
