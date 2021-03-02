using System;
using System.Linq;
using ImagineCommunications.Gameplan.Synchronization.Objects;

namespace ImagineCommunications.Gameplan.Synchronization
{
    /// <summary>
    /// The synchronization token.
    /// </summary>
    public struct SynchronizationToken : IEquatable<SynchronizationToken>
    {
        public static readonly SynchronizationToken Empty = Create();

        public SynchronizationObjectToken Object { get; }

        public SynchronizationObjectOwnerToken Owner { get; }

        private SynchronizationToken(SynchronizationObject synchronizationObject, string ownerId)
        {
            if (synchronizationObject != null)
            {
                if (!synchronizationObject.ServiceId.HasValue)
                {
                    throw new ArgumentException($"Synchronization object is free. SynchronizationObject.Id = '{synchronizationObject.Id}'");
                }

                var owner = synchronizationObject.Owners.SingleOrDefault(oo => oo.OwnerId == ownerId && oo.IsActive);
                if (owner == null)
                {
                    throw new InvalidOperationException($"Synchronization object owner not found. SynchronizationObject.Id = '{synchronizationObject.Id}', Owner.Id = '{ownerId}'");
                }

                Object = SynchronizationObjectToken.Create(synchronizationObject.Id, synchronizationObject.ServiceId.Value);
                Owner = SynchronizationObjectOwnerToken.Create(owner.Id, owner.OwnerId);
            }
            else
            {
                Object = SynchronizationObjectToken.Empty;
                Owner = SynchronizationObjectOwnerToken.Empty;
            }
        }

        public static SynchronizationToken Create(SynchronizationObject synchronizationObject, string ownerId) =>
            new SynchronizationToken(synchronizationObject, ownerId);

        private static SynchronizationToken Create() => new SynchronizationToken(null, null);

        public override string ToString() => $"[Object:{Object}][Owner:{Owner}]";

        public bool Equals(SynchronizationToken other) => Object == other.Object && Owner == other.Owner;

        public override bool Equals(object other)
        {
            if (other is SynchronizationToken token)
            {
                return Equals(token);
            }

            return false;
        }

        public override int GetHashCode() => SynchronizationExtensions.GetHashCode(Object, Owner);

        public static bool operator !=(SynchronizationToken x, SynchronizationToken y) => !(x == y);

        public static bool operator ==(SynchronizationToken x, SynchronizationToken y) => x.Equals(y);
    }
}
