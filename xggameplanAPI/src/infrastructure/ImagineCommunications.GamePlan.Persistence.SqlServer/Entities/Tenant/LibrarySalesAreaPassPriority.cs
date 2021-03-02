using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class LibrarySalesAreaPassPriority : IUniqueIdentifierPrimaryKey, IAuditEntity
    {
        Guid ISinglePrimaryKey<Guid>.Id
        {
            get => Uid;
            set => Uid = value;
        }

        public int Id { get; set; }

        public Guid Uid { get; set; }

        public string Name { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public SortedSet<DayOfWeek> DowPattern { get; set; } = new SortedSet<DayOfWeek>();

        public List<SalesAreaPriority> SalesAreaPriorities { get; set; }
    }
}
