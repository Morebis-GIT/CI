using System;
using xggameplan.AuditEvents;

namespace xggameplan.core.AuditEvents.AuditEvents
{
    internal class PreRunRecalculateBreakAvailabilityEndAuditEvent
        : AuditEvent
    {
        public PreRunRecalculateBreakAvailabilityEndAuditEvent()
        {
            ID = Guid.NewGuid().ToString();
            TimeCreated = DateTime.UtcNow;
            EventTypeID = AuditEventTypes.GamePlanRecalculateBreakAvailability;

            Values.Add(
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanPipelineEventID,
                    PipelineEventIDs.FINISHED_PRE_RUN_RECALCULATING_BREAK_AVAILABILITY
                )
            );
        }

        public PreRunRecalculateBreakAvailabilityEndAuditEvent(int tenantId, int userId)
            : this()
        {
            TenantID = tenantId;
            UserID = userId;
        }
    }
}
