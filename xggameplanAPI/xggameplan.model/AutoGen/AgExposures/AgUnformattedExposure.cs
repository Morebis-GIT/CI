using System;

namespace xggameplan.Model.AutoGen
{
    public class AgUnformattedExposure
    {
        public int BreakSalesAreaNo { get; set; }

        public string ClashCode { get; set; }

        public string MasterClashCode { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int StartDay { get; set; }

        public int EndDay { get; set; }

        public int NoOfExposures { get; set; }
    }
}
