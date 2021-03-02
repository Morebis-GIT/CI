namespace ImagineCommunications.Gameplan.Synchronization.Interfaces
{
    /// <summary>
    /// The synchronization service.
    /// </summary>
    public interface ISynchronizationService
    {
        /// <summary>
        /// Tries to capture synchronization lock.
        /// </summary>
        /// <param name="synchronizationServiceId">The synchronization service Id.</param>
        /// <param name="ownerId">The owner Id.</param>
        /// <param name="token">The synchronization token.</param>
        /// <returns>Returns <see langword="true"/> if lock was captured. Otherwise - <see langword="false"/>.</returns>
        bool TryCapture(int synchronizationServiceId, string ownerId, out SynchronizationToken token);

        /// <summary>
        /// Tries to release synchronization lock.
        /// </summary>
        /// <param name="token">The synchronization token.</param>
        /// <param name="throwIfAlreadyReleased">The value indicating whether the exception should be thrown if lock already released.</param>
        void Release(SynchronizationToken token, bool throwIfAlreadyReleased = false);

        /// <summary>
        /// Tries to release synchronization lock.
        /// </summary>
        /// <param name="ownerId">The owner Id.</param>
        /// <param name="throwIfAlreadyReleased">The value indicating whether the exception should be thrown if lock already released.</param>
        void Release(string ownerId, bool throwIfAlreadyReleased = false);
    }
}
