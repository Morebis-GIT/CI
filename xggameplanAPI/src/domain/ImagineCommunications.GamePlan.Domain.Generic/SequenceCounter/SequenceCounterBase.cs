using System;
using System.Threading;

namespace ImagineCommunications.GamePlan.Domain.Generic.SequenceCounter
{
    /// <summary>
    /// Exposes base class for <see cref="ISequenceCounter{T}"/> implementation.
    /// </summary>
    public abstract class SequenceCounterBase<T> : ISequenceCounter<T>
        where T : ICounterValue
    {
        protected abstract ref int GetCounter(T obj);

        public void Process(T obj)
        {
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var v = Interlocked.Increment(ref GetCounter(obj));
            obj.Value = v;
        }
    }
}
