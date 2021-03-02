using System;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Clash
{
    public class ClashDifference
    {
        public ClashDifference(string salesArea, DateTime? startDate, DateTime? endDate, int? peakExposureCount, int? offPeakExposureCount)
        {
            SalesArea = salesArea;
            StartDate = startDate;
            EndDate = endDate;
            PeakExposureCount = peakExposureCount;
            OffPeakExposureCount = offPeakExposureCount;
        }

        public string SalesArea { get; }

        public DateTime? StartDate { get; }

        public DateTime? EndDate { get; }

        public int? PeakExposureCount { get; }

        public int? OffPeakExposureCount { get; }
    }
}
