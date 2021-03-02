namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    /// <summary>
    /// Exposes a property that can provide a memory repository with a value to
    /// compartmentalise objects of the same type.
    /// </summary>
    internal interface IMemoryRepositoryCompartment
    {
        /// <summary>
        /// A key used to compartmentalise objects of the same type in the
        /// repository. This may be needed when using a single memory repository
        /// in unit tests and do not want their data to leak into other tests.
        /// </summary>
        string CompartmentKey { get; set; }
    }
}
