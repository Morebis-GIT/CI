using System;
using System.Collections.Generic;
using NodaTime;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas
{
    /// <summary>
    /// Represents a Sales Area within GamePlan
    /// </summary>
    public class SalesArea
    {
        public int Uid { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string CurrencyCode { get; set; }
        public string BaseDemographic1 { get; set; }
        public string BaseDemographic2 { get; set; }
        public IEnumerable<string> ChannelGroup { get; set; }
        public Duration StartOffset { get; set; }
        public Duration DayDuration { get; set; }
    }
}
