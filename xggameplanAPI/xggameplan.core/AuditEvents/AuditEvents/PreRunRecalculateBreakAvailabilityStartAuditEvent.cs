using System;
using xggameplan.AuditEvents;

namespace xggameplan.core.AuditEvents.AuditEvents
{
    internal class PreRunRecalculateBreakAvailabilityStartAuditEvent
        : AuditEvent
    {
        public PreRunRecalculateBreakAvailabilityStartAuditEvent()
        {
            ID = Guid.NewGuid().ToString();
            TimeCreated = DateTime.UtcNow;
            EventTypeID = AuditEventTypes.GamePlanRecalculateBreakAvailability;

            Values.Add(
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanPipelineEventID,
                    PipelineEventIDs.STARTED_PRE_RUN_RECALCULATING_BREAK_AVAILABILITY
                )
            );
        }

        public PreRunRecalculateBreakAvailabilityStartAuditEvent(int tenantId, int userId)
            : this()
        {
            TenantID = tenantId;
            UserID = userId;
        }
    }
}
