namespace ImagineCommunications.GamePlan.Domain.Shared.Schedules
{
    /// <summary>
    /// Represents a unique index for Schedule entity/model,
    /// usually is calculated by couple of Date and SalesArea values.
    /// </summary>
    public interface IScheduleIndex
    {
        int ScheduleUniqueKey { get; }
    }
}
