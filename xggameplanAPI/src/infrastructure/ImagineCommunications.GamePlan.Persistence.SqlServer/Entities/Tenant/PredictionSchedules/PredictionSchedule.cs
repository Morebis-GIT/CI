using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules
{
    public class PredictionSchedule : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public string SalesArea { get; set; }
        public DateTime ScheduleDay { get; set; }

        public ICollection<PredictionScheduleRating> Ratings { get; set; }

    }
}
