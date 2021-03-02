using System.Collections.Concurrent;
using ImagineCommunications.GamePlan.Domain.Generic.SequenceCounter;

namespace ImagineCommunications.GamePlan.Domain.Shared.Programmes.Counters
{
    /// <summary>
    /// Exposes sequence counter functionality for programme entities/models based on
    /// <see cref="IProgrammePrgtNoCounter"/> interface.
    /// </summary>
    public class ProgrammePrgtNoSequenceCounter : SequenceCounterBase<IProgrammePrgtNoCounter>
    {
        private readonly ConcurrentDictionary<int, CounterStorage> _counters =
            new ConcurrentDictionary<int, CounterStorage>();

        protected override ref int GetCounter(IProgrammePrgtNoCounter obj)
        {
            var key = obj.ScheduleUniqueKey;
            var cStorage = _counters.GetOrAdd(key, x => new CounterStorage());
            return ref cStorage.Counter;
        }

        protected class CounterStorage
        {
            public int Counter;
        }
    }
}
