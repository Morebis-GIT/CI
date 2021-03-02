using System;
using xggameplan.AuditEvents;

namespace xggameplan.core.Helpers
{
    public class PipelineEventHelper
    {
        public static PipelineAuditEvent CreatePipelineAuditEvent(int eventTypeId, int eventId, Guid runId, Guid scenarioId, string message)
        {
            var pipelineAuditEvent = new PipelineAuditEvent
            {
                TimeCreated = DateTime.UtcNow,
                EventTypeId = eventTypeId,
                EventId = eventId,
                RunId = runId,
                ScenarioId = scenarioId,
                Message = string.IsNullOrWhiteSpace(message) ? string.Empty : message
            };

            return pipelineAuditEvent;
        }
    }
}
