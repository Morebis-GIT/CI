using System;

namespace xggameplan.Model
{
    public class RecommendationExtendedModel : RecommendationModel
    {
        /// <summary>
        /// Sales Area Group Name
        /// </summary>
        public string SalesAreaGroupName { get; set; }

        /// <summary>
        /// Demographic Name
        /// </summary>
        public string DemographicName { get; set; }

        /// <summary>
        /// Client Name
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Parent Clash Name
        /// </summary>
        public string ParentClashName { get; set; }

        /// <summary>
        /// Clash Name
        /// </summary>
        public string ClashName { get; set; }

        /// <summary>
        /// Product Name
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// StartTime
        /// </summary>
        public TimeSpan StartTime { get; set; }

        /// <summary>
        /// EndDate
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// EndTime
        /// </summary>
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// StartDayOfWeek in tenant settings
        /// </summary>
        public string StartDayOfWeek { get; set; }

        /// <summary>
        /// 'Peak' / 'Off Peak' / 'Midnight to Dawn' which Spot startTime will be in
        /// </summary>
        public string DayPart { get; set; }

        /// <summary>
        /// WeekCommencingDate of the spot
        /// </summary>
        public DateTime WeekCommencingDate { get; set; }

        /// <summary>
        /// SpotLength duration in total seconds
        /// </summary>
        public double SpotLengthInSec { get; set; }
    }
}
