namespace ImagineCommunications.Gameplan.Synchronization
{
    /// <summary>
    /// Describes possible type of services to synchronize.
    /// </summary>
    /// <remarks>
    /// This service is public static instead of regular enum because in future synchronization service can be used for other services and
    /// service types Ids can be declared in different places of solution. Not in one enum. That is why int is used as service type Id and
    /// public static readonly int is used to avoid explicit cast from enum type to int type. 
    /// </remarks>
    public static class SynchronizedServiceType
    {
        public static readonly int RunExecution = 1;
        public static readonly int DataSynchronization = 2;
    }
}
