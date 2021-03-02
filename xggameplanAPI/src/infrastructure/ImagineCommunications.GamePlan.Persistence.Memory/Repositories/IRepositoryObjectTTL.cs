namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    internal interface IRepositoryObjectTTL
    {
        /// <summary>
        /// <para>
        /// Sets a value that is used to indicate when a memory repository entry
        /// should be evicted from its cache. Memory repository objects are
        /// designed to be transient and should be persisted to more long-lived
        /// storage as soon as possible. As the objects exist only in memory,
        /// pressure of resources may force old entries to be flushed.
        /// </para>
        /// <para>
        /// The default object TTL is one day. Use this property after
        /// retrieving a repository reference to adjust item TTLs for that
        /// repository alone. Further, because the object's TTL is set via this
        /// property's current value you can adjust it on the fly for different objects.
        /// </para>
        /// </summary>
        double ObjectTTLMilliseconds { set; }

        /// <summary>
        /// Resets the repository's TTL to one day.
        /// </summary>
        void ResetObjectTTLMilliseconds();
    }
}
