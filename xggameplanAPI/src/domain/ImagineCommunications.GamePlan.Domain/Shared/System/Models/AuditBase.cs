using System;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Models
{
    public abstract class AuditBase
    {
        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }
    }
}
