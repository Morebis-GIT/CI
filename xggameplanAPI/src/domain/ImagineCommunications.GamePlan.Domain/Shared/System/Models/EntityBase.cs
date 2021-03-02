using System;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Models
{
    public abstract class EntityBase : AuditBase
    {
        public int Id { get; set; }

        public Guid Uid { get; set; }
    }

}
