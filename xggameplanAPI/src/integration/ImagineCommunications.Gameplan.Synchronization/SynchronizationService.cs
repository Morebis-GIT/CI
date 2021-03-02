using System;
using System.Linq;
using ImagineCommunications.Gameplan.Synchronization.Exceptions;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;

namespace ImagineCommunications.Gameplan.Synchronization
{
    /// <inheritdoc />
    public class SynchronizationService : ISynchronizationService
    {
        private static readonly int MaxRetriesNumber = int.MaxValue;

        private readonly ISynchronizationObjectRepository _synchronizationObjectRepository;
        private readonly SynchronizationServicesConfiguration _configuration;

        public SynchronizationService(
            ISynchronizationObjectRepository synchronizationObjectRepository,
            SynchronizationServicesConfiguration configuration)
        {
            _synchronizationObjectRepository = synchronizationObjectRepository;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public bool TryCapture(int synchronizationServiceId, string ownerId, out SynchronizationToken token)
        {
            var synchronizationService = GetSynchronizationService(synchronizationServiceId);

            var triesCount = 0;
            while (triesCount < MaxRetriesNumber)
            {
                try
                {
                    return TryCaptureInternal(synchronizationService, ownerId, out token);
                }
                catch (DbConcurrencyException)
                {
                    _synchronizationObjectRepository.DiscardChanges();
                }

                triesCount++;
            }

            token = SynchronizationToken.Empty;

            return false;
        }

        /// <inheritdoc />
        public void Release(SynchronizationToken token, bool throwIfAlreadyReleased = false)
        {
            var triesCount = 0;
            while (triesCount < MaxRetriesNumber)
            {
                try
                {
                    ReleaseInternal(token, throwIfAlreadyReleased);

                    return;
                }
                catch (DbConcurrencyException)
                {
                    // Exception throwing means that synchronization object was updated by another execution:
                    // 1. Another execution decremented Owner Count of this sync object
                    // 2. (Wrong using) Service with the same type released sync object on purpose (we don't handle owners validation very strong, only owner type comparison)

                    _synchronizationObjectRepository.DiscardChanges();

                    // In happy case:
                    // Current synchronization state isn't updated because of parallel updating exception occurring
                    // Current execution should decrement the count of owners of the sync object
                }

                triesCount++;
            }
        }

        /// <inheritdoc />
        public void Release(string ownerId, bool throwIfAlreadyReleased = false)
        {
            var token = Get(ownerId);
            if (token == SynchronizationToken.Empty)
            {
                throw new InvalidOperationException($"Incorrect synchronization object. Owner.Id = '{ownerId}'");
            }

            Release(token, throwIfAlreadyReleased);
        }

        private SynchronizationToken Get(string ownerId)
        {
            var synchronizationObject = _synchronizationObjectRepository.GetActiveByOwnerId(ownerId);
            var token = synchronizationObject == null
                ? SynchronizationToken.Empty
                : SynchronizationToken.Create(synchronizationObject, ownerId);

            return token;
        }

        private bool TryCaptureInternal(
            SynchronizationServiceConfiguration synchronizationService,
            string ownerId,
            out SynchronizationToken token)
        {
            var synchronizationObject = _synchronizationObjectRepository.GetLastCreated();

            // If the synchronization object isn't captured
            if (!synchronizationObject.ServiceId.HasValue)
            {
                synchronizationObject = _synchronizationObjectRepository.Capture(synchronizationObject.Id, ownerId, synchronizationService.Id);
                _synchronizationObjectRepository.SaveChanges();

                // Exception throwing means that synchronization object was updated (captured) by another service execution

                token = SynchronizationToken.Create(synchronizationObject, ownerId);

                return true;
            }

            if (synchronizationObject.ServiceId == synchronizationService.Id)
            {
                if (synchronizationService.MaxConcurrencyLevel.HasValue &&
                    synchronizationObject.OwnerCount == synchronizationService.MaxConcurrencyLevel)
                {
                    token = SynchronizationToken.Empty;

                    return false;
                }

                if (synchronizationObject.Owners.Any(oo => oo.OwnerId == ownerId && oo.IsActive))
                {
                    token = SynchronizationToken.Empty;

                    return false;
                }

                synchronizationObject = _synchronizationObjectRepository.Capture(synchronizationObject.Id, ownerId);
                _synchronizationObjectRepository.SaveChanges();

                // Exception throwing means that synchronization object was updated by another run execution, there are only 3 cases:
                // 1. Another new incoming run incremented Count of Owners (Captured this object) of this sync object
                // 2. Some owner of this sync object decremented Count of Owners
                // 3. Some owner of this sync object decremented Count of Owners and in case if CountOfOwners == 0 released this object and added new entry to the database

                token = SynchronizationToken.Create(synchronizationObject, ownerId);

                return true;
            }

            // If synchronization object is captured by another service

            token = SynchronizationToken.Empty;

            return false;
        }

        private void ReleaseInternal(SynchronizationToken token, bool throwIfAlreadyReleased)
        {
            var synchronizationObject = _synchronizationObjectRepository.GetById(token.Object.Id, token.Owner.Id);
            if (synchronizationObject == null)
            {
                throw new InvalidOperationException($"Synchronization object not found. SynchronizationObject.Id = '{token.Object.Id}', Owner.Id = '{token.Owner.Id}'");
            }

            if (synchronizationObject.OwnerCount == 0 && throwIfAlreadyReleased)
            {
                throw new InvalidOperationException($"Can not release synchronization object that is already released. SynchronizationObject.Id = '{token.Object.Id}', Owner.Id = '{token.Owner.Id}'");
            }

            var owner = synchronizationObject.Owners.SingleOrDefault(o => o.Id == token.Owner.Id);
            if (owner == null)
            {
                throw new InvalidOperationException($"Synchronization object owner not found. SynchronizationObject.Id = '{token.Object.Id}', Owner.Id = '{token.Owner.Id}'");
            }

            if (!owner.IsActive && throwIfAlreadyReleased)
            {
                throw new InvalidOperationException($"Synchronization object owner is not active. SynchronizationObject.Id = '{token.Object.Id}', Owner.Id = '{token.Owner.Id}'");
            }

            _synchronizationObjectRepository.Release(token.Object.Id, token.Owner.OwnerId);
            _synchronizationObjectRepository.SaveChanges();
        }

        private SynchronizationServiceConfiguration GetSynchronizationService(int id)
        {
            if (!_configuration.TryGet(id, out var synchronizationService))
            {
                throw new InvalidOperationException($"Synchronization service with Id '{id}' not found");
            }

            return synchronizationService;
        }
    }
}
