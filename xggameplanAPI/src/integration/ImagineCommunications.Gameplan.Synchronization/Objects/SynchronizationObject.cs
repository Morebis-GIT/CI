using System;
using System.Collections.Generic;

namespace ImagineCommunications.Gameplan.Synchronization.Objects
{
    public class SynchronizationObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public int? ServiceId { get; set; }

        public byte[] RowVersion { get; set; }

        public int OwnerCount { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<SynchronizationObjectOwner> Owners { get; set; } = new HashSet<SynchronizationObjectOwner>();
    }
}
