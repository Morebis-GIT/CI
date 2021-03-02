using System;
using NodaTime;

namespace xggameplan.Model
{
    public class SpotWithBreakAndProgrammeInfo
    {

        public string ProgrammeName { get; set; }

        public DateTime? ProgrammeStartTime { get; set; }

        public Duration? ProgrammeDuration { get; set; }

        public DateTime? BreakStartTime { get; set; }
        public Duration? BreakDuration { get; set; }

        public double? Efficiency { get; set; }

        public string ExternalBreakNo { get; set; }
        public string ExternalCampaignNumber { get; set; }
        public string CampaignGroup { get; set; }
        public string CampaignName { get; set; }
        public Duration? SpotLength { get; set; }
        public string ProductName { get; set; }
        public string Demographic { get; set; }
        public string AdvertiserName { get; set; }
        public string ClashCode { get; set; }
        public string ClashDescription { get; set; }
    }
}
