using System;

namespace xggameplan.AuditEvents
{
    public class AuditEventValueType
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public Type Type { get; set; }
        public bool Internal { get; set; }

        public AuditEventValueType(int id, string description, Type type, bool isInternal)
        {
            ID = id;
            Description = description;
            Type = type;
            Internal = isInternal;
        }
    }
}
