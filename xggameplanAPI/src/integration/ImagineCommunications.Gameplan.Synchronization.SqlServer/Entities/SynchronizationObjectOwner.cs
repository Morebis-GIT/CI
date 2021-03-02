using System;

namespace ImagineCommunications.Gameplan.Synchronization.SqlServer.Entities
{
    public class SynchronizationObjectOwner
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid SynchronizationObjectId { get; set; }

        public string OwnerId { get; set; }

        public bool IsActive { get; set; }

        public DateTime CapturedDate { get; set; }

        public DateTime? ReleasedDate { get; set; }
    }
}
