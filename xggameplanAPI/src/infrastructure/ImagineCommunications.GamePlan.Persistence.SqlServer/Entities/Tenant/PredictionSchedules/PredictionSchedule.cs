using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules
{
    public class PredictionSchedule : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid SalesAreaId { get; set; }
        public DateTime ScheduleDay { get; set; }

        public ICollection<PredictionScheduleRating> Ratings { get; set; }
        public SalesArea SalesArea { get; set; }
    }
}
