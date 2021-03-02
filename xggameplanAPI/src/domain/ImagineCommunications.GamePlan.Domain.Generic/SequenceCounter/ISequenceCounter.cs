namespace ImagineCommunications.GamePlan.Domain.Generic.SequenceCounter
{
    /// <summary>
    /// Exposes functionality of sequence counter based on incrementing <see cref="ICounterValue.Value"/> property.
    /// </summary>
    public interface ISequenceCounter<in T>
        where T : ICounterValue
    {
        void Process(T obj);
    }
}
