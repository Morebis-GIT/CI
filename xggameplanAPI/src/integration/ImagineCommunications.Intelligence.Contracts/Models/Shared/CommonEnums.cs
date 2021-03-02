namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public enum HolidayType
    {
        PublicHoliday = 0,
        SchoolHoliday = 1
    }

    public enum ClashExceptionType
    {
        Clash,
        Product,
        Advertiser
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
}
