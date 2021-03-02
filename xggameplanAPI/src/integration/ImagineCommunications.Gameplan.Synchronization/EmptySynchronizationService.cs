using ImagineCommunications.Gameplan.Synchronization.Interfaces;

namespace ImagineCommunications.Gameplan.Synchronization
{
    public class EmptySynchronizationService : ISynchronizationService
    {
        /// <inheritdoc />
        public bool TryCapture(int synchronizationServiceId, string ownerId, out SynchronizationToken token)
        {
            token = SynchronizationToken.Empty;

            return true;
        }

        /// <inheritdoc />
        public void Release(SynchronizationToken token, bool throwIfAlreadyReleased = false)
        {
        }

        /// <inheritdoc />
        public void Release(string ownerId, bool throwIfAlreadyReleased = false)
        {
        }
    }
}
