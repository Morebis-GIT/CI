using System;
using System.Collections.Generic;
using xgCore.xgGamePlan.ApiEndPoints.Models.Breaks;
using xgCore.xgGamePlan.ApiEndPoints.Models.Programmes;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Schedules
{
    public class Schedule
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string SalesArea { get; set; }
        public IEnumerable<Programme> Programmes { get; set; }
        public IEnumerable<Break> Breaks { get; set; }
    }
}
