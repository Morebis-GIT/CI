using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules
{
    public class PredictionScheduleRating : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int PredictionScheduleId { get; set; }
        public DateTime Time { get; set; }
        public string Demographic { get; set; }
        public double NoOfRatings { get; set; }

        public PredictionSchedule PredictionSchedule { get; set; }
    }
}
