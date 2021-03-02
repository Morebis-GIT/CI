using System;

namespace ImagineCommunications.Gameplan.Synchronization
{
    /// <summary>
    /// The synchronization object owner token.
    /// </summary>
    public struct SynchronizationObjectOwnerToken : IEquatable<SynchronizationObjectOwnerToken>
    {
        public static readonly SynchronizationObjectOwnerToken Empty = Create();

        public Guid Id { get; }

        public string OwnerId { get; }

        private SynchronizationObjectOwnerToken(Guid id, string ownerId)
        {
            Id = id;
            OwnerId = ownerId;
        }

        public static SynchronizationObjectOwnerToken Create(Guid id, string ownerId) => new SynchronizationObjectOwnerToken(id, ownerId);

        private static SynchronizationObjectOwnerToken Create() => new SynchronizationObjectOwnerToken(Guid.Empty, null);

        public override string ToString() => $"[Id:{Id}][OwnerId:{OwnerId}]";

        public bool Equals(SynchronizationObjectOwnerToken other) => Id == other.Id && OwnerId == other.OwnerId;

        public override bool Equals(object other)
        {
            if (other is SynchronizationObjectOwnerToken token)
            {
                return Equals(token);
            }

            return false;
        }

        public override int GetHashCode() => SynchronizationExtensions.GetHashCode(Id, OwnerId);

        public static bool operator !=(SynchronizationObjectOwnerToken x, SynchronizationObjectOwnerToken y) => !(x == y);

        public static bool operator ==(SynchronizationObjectOwnerToken x, SynchronizationObjectOwnerToken y) => x.Equals(y);
    }
}
