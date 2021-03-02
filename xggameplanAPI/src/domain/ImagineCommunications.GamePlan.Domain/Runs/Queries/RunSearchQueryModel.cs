using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;

namespace ImagineCommunications.GamePlan.Domain.Runs.Queries
{
    public class RunSearchQueryModel : BaseQueryModel
    {
        public DateTime? RunPeriodStartDate { get; set; }
        public DateTime? RunPeriodEndDate { get; set; }
        public IEnumerable<int> Users { get; set; }
        public string Description { get; set; }
        public IEnumerable<RunStatus> Status { get; set; }
        public DateTime? ExecutedStartDate { get; set; }
        public DateTime? ExecutedEndDate { get; set; }
        public List<Order<string>> Orderby { get; set; }
    }
}
