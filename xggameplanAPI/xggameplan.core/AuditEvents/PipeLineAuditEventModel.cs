using System;

namespace xggameplan.AuditEvents
{
    public class PipelineAuditEventModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Time created.
        /// </summary>
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// Event type ID.
        /// </summary>
        public int EventTypeId { get; set; }

        /// <summary>
        /// Event ID.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Run Id.
        /// </summary>
        public Guid RunId { get; set; }

        /// <summary>
        /// Scenario Id.
        /// </summary>
        public Guid ScenarioId { get; set; }

        /// <summary>
        /// Message.
        /// </summary>
        public String Message { get; set; }
    }
}
