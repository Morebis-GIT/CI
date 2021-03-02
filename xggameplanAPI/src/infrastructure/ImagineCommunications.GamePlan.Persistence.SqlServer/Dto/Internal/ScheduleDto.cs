using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Dto.Internal
{
    internal class ScheduleDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string SalesArea { get; set; }
        public List<Break> Breaks = new List<Break>();
        public List<Programme> Programmes = new List<Programme>();
    }
}
