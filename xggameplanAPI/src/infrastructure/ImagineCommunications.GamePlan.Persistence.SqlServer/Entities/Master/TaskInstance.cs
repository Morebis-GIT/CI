using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master
{
    public class TaskInstance : IUniqueIdentifierPrimaryKey
    {
        public Guid Id { get; set; }

        public string TaskId { get; set; }

        public int TenantId { get; set; }

        public int Status { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime TimeCompleted { get; set; }

        public DateTime TimeLastActive { get; set; }

        public List<TaskInstanceParameter> Parameters { get; set; }
    }
}
