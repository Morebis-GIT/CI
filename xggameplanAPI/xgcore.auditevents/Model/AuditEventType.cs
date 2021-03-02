namespace xggameplan.AuditEvents
{
    public class AuditEventType
    {
        public int ID { get; set; }
        public string Description { get; set; }

        public AuditEventType(int id, string description)
        {
            ID = id;
            Description = description;
        }
    }
}
