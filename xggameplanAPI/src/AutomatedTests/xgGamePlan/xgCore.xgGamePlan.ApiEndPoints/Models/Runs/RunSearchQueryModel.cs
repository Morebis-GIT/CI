using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Runs
{
    public class RunSearchQueryModel
    {
        public DateTime? RunPeriodStartDate { get; set; }
        public DateTime? RunPeriodEndDate { get; set; }
        public IEnumerable<int> Users { get; set; }
        public string Description { get; set; }
        public IEnumerable<RunStatus> Status { get; set; }
        public DateTime? ExecutedStartDate { get; set; }
        public DateTime? ExecutedEndDate { get; set; }
        public IEnumerable<Order<string>> Orderby { get; set; }
    }
}
