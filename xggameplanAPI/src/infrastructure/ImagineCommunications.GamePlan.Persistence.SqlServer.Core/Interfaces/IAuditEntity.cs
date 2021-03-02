using System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces
{
    public interface IAuditEntity
    {
        DateTime DateCreated { get; set; }

        DateTime DateModified { get; set; }
    }
}
