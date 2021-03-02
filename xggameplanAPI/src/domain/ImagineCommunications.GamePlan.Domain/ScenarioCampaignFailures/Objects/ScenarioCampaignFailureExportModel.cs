using System;

namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects
{
    public class ScenarioCampaignFailureExportModel
    {
        public string ExternalCampaignId { get; set; }
        public string CampaignName { get; set; }
        public string SalesAreaGroupName { get; set; }
        public string SalesAreaName { get; set; }
        public int DurationSecs { get; set; }
        public int MultipartNo { get; set; }
        public DateTime StrikeWeightStartDate { get; set; }
        public DateTime StrikeWeightEndDate { get; set; }
        public TimeSpan DayPartStartTime { get; set; }
        public TimeSpan DayPartEndTime { get; set; }
        public string DayPartDays { get; set; }
        public string FailureTypeName { get; set; }
        public long FailureCount { get; set; }
        public string PassesEncounteringFailure { get; set; }
    }
}
