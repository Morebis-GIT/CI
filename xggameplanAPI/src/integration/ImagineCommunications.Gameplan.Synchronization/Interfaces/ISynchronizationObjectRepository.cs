using System;
using ImagineCommunications.Gameplan.Synchronization.Objects;

namespace ImagineCommunications.Gameplan.Synchronization.Interfaces
{
    public interface ISynchronizationObjectRepository
    {
        SynchronizationObject GetLastCreated();

        SynchronizationObject GetById(Guid id, Guid ownerObjectId);

        SynchronizationObject GetActiveByOwnerId(string ownerId);

        SynchronizationObject Capture(Guid synchronizationObjectId, string ownerId, int? serviceId = null);

        void Release(Guid synchronizationObjectId, string ownerId);

        void SaveChanges();

        void DiscardChanges();
    }
}
