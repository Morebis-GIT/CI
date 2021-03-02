using System;

namespace ImagineCommunications.Gameplan.Synchronization
{
    /// <summary>
    /// The synchronization object token.
    /// </summary>
    public struct SynchronizationObjectToken : IEquatable<SynchronizationObjectToken>
    {
        public static readonly SynchronizationObjectToken Empty = Create();

        public Guid Id { get; }

        public int ServiceId { get; }

        private SynchronizationObjectToken(Guid id, int serviceId)
        {
            Id = id;
            ServiceId = serviceId;
        }

        internal static SynchronizationObjectToken Create(Guid id, int serviceId) => new SynchronizationObjectToken(id, serviceId);

        private static SynchronizationObjectToken Create() => new SynchronizationObjectToken(Guid.Empty, 0);

        public override string ToString() => $"[Id:{Id}][ServiceId:{ServiceId}]";

        public bool Equals(SynchronizationObjectToken other) => Id == other.Id && ServiceId == other.ServiceId;

        public override bool Equals(object other)
        {
            if (other is SynchronizationObjectToken token)
            {
                return Equals(token);
            }

            return false;
        }

        public override int GetHashCode() => SynchronizationExtensions.GetHashCode(Id, ServiceId);

        public static bool operator !=(SynchronizationObjectToken x, SynchronizationObjectToken y) => !(x == y);

        public static bool operator ==(SynchronizationObjectToken x, SynchronizationObjectToken y) => x.Equals(y);
    }
}
