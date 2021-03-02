using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Audit
{
    internal class AuditEntityHelper
    {
        public static void SetAudit(IAuditEntity entity, AuditEntityState state, DateTime timestamp)
        {
            if (entity is null)
            {
                return;
            }
            if (state != AuditEntityState.None)
            {
                if (state == AuditEntityState.Added)
                {
                    entity.DateCreated = timestamp;
                }

                entity.DateModified = timestamp;
            }
        }
    }
}
