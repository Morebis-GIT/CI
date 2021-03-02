using System;
using System.Collections.Generic;

namespace xggameplan.AuditEvents
{
    public class AuditEvent
    {
        public string ID { get; set; } = String.Empty;

        /// <summary>
        /// Time created.
        /// </summary>
        public DateTime TimeCreated { get; set; }

        /// <summary>
        /// Event type.
        /// </summary>
        public int EventTypeID { get; set; }

        /// <summary>
        /// Tenant (0 = Unknown).
        /// </summary>
        public int TenantID { get; set; }

        /// <summary>
        /// User (0 = Unknown)
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Source
        /// </summary>
        public string Source { get; set; }

        public List<AuditEventValue> Values { get; } = new List<AuditEventValue>();

        public AuditEventValue GetValueByValueTypeId(int valueTypeId) =>
            Values.Find(value => value.TypeID == valueTypeId);
    }
}
