namespace ImagineCommunications.GamePlan.Domain.BusinessRules.ClashExceptions
{
    public enum ClashExceptionType
    {
        Clash,
        Product,
        Advertiser
    }

    public enum ClashExceptionOrder
    {
        StartDate,
        EndDate,
        FromType,
        ToType,
        FromValue,
        ToValue,
        IncludeOrExclude
    }
}
