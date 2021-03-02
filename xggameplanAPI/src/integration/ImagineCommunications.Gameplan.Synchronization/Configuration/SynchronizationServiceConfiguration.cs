namespace ImagineCommunications.Gameplan.Synchronization
{
    public sealed class SynchronizationServiceConfiguration
    {
        public int Id { get; }
        public int? MaxConcurrencyLevel { get; }

        public SynchronizationServiceConfiguration(int id, int? maxConcurrencyLevel = null)
        {
            Id = id;
            MaxConcurrencyLevel = maxConcurrencyLevel;
        }
    }
}
