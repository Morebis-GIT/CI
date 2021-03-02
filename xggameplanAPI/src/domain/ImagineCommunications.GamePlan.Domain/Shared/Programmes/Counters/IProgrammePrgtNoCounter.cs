using ImagineCommunications.GamePlan.Domain.Generic.SequenceCounter;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;

namespace ImagineCommunications.GamePlan.Domain.Shared.Programmes.Counters
{
    /// <summary>
    /// Represents <see cref="ICounterValue"/> functionality for generation value of Programme.PrgtNo property
    /// </summary>
    public interface IProgrammePrgtNoCounter : ICounterValue, IScheduleIndex
    {
    }
}
