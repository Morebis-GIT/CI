namespace ImagineCommunications.Gameplan.Integration.Contracts.Shared.Enums
{
    public enum DeliveryCurrency
    {
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
}
