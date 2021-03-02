using System;
using xggameplan.AuditEvents;

namespace xggameplan.core.AuditEvents.AuditEvents
{
    internal class PostSmoothRecalculateBreakAvailabilityStartAuditEvent
        : AuditEvent
    {
        public PostSmoothRecalculateBreakAvailabilityStartAuditEvent()
        {
            ID = Guid.NewGuid().ToString();
            TimeCreated = DateTime.UtcNow;
            EventTypeID = AuditEventTypes.GamePlanRecalculateBreakAvailability;

            Values.Add(
                new AuditEventValue(
                    AuditEventValueTypes.GamePlanPipelineEventID,
                    PipelineEventIDs.STARTED_POST_SMOOTH_RECALCULATING_BREAK_AVAILABILITY
                )
            );
        }

        public PostSmoothRecalculateBreakAvailabilityStartAuditEvent(int tenantId, int userId)
            : this()
        {
            TenantID = tenantId;
            UserID = userId;
        }
    }
}
