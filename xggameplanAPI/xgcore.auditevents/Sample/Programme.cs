using System;

namespace xggameplan.AuditEvents.Sample
{
    public class Programme
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public override string ToString()
        {
            return string.Format("Id={0}; Name={1}; StartDate={2}, EndDate={3}", Id, Name, StartDate, EndDate);
        }
    }
}
