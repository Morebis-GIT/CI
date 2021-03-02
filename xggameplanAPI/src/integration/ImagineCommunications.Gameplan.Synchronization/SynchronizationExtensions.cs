using System;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;

namespace ImagineCommunications.Gameplan.Synchronization
{
    public static class SynchronizationExtensions
    {
        public static bool TryCapture(
            this ISynchronizationService synchronizationService,
            int synchronizationServiceId,
            Guid ownerId,
            out SynchronizationToken token)
        {
            return synchronizationService.TryCapture(synchronizationServiceId, ownerId.ToString("D").ToLowerInvariant(), out token);
        }

        public static void Release(this ISynchronizationService synchronizationService, Guid ownerId)
        {
            synchronizationService.Release(ownerId.ToString("D").ToLowerInvariant());
        }

        public static int GetHashCode<T1, T2>(T1 t1, T2 t2)
        {
            return (t1.GetHashCode() * 397) ^ (t2.GetHashCode() * 31);
        }
    }
}
