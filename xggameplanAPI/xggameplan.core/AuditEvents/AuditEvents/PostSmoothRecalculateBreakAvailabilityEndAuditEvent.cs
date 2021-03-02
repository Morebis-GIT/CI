using System;
using xggameplan.AuditEvents;

namespace xggameplan.core.AuditEvents.AuditEvents
{
    internal class PostSmoothRecalculateBreakAvailabilityEndAuditEvent
        : AuditEvent
    {
        public PostSmoothRecalculateBreakAvailabilityEndAuditEvent()
        {
            ID = Guid.NewGuid().ToString();
            TimeCreated = DateTime.UtcNow;
            EventTypeID = AuditEventTypes.GamePlanRecalculateBreakAvailability;

            Values.Add(
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanPipelineEventID,
                    PipelineEventIDs.FINISHED_POST_SMOOTH_RECALCULATING_BREAK_AVAILABILITY
                )
            );
        }

        public PostSmoothRecalculateBreakAvailabilityEndAuditEvent(int tenantId, int userId)
            : this()
        {
            TenantID = tenantId;
            UserID = userId;
        }
    }
}
