using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Programmes
{
    public class Programme
    {
        public string Id { get; set; }
        public int PrgtNo { get; set; }
        public string SalesArea { get; set; }
        public DateTime StartDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string ExternalReference { get; set; }
        public string ProgrammeName { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> ProgrammeCategories { get; set; }
        public string Classification { get; set; }
        public bool LiveBroadcast { get; set; }
    }
}
