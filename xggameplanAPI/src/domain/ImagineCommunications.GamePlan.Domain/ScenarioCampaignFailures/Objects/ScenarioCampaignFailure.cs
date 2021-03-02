using System;
using NodaTime;

namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects
{
    public class ScenarioCampaignFailure
    {
        public int Id { get; set; }
        public Guid ScenarioId { get; set; }
        public string ExternalCampaignId { get; set; }
        public string SalesAreaGroup { get; set; }
        public string SalesArea { get; set; }
        public Duration Length { get; set; }
        public int MultipartNo { get; set; }
        public DateTime StrikeWeightStartDate { get; set; }
        public DateTime StrikeWeightEndDate { get; set; }
        public TimeSpan DayPartStartTime { get; set; }
        public TimeSpan DayPartEndTime { get; set; }
        public string DayPartDays { get; set; }
        public int FailureType { get; set; }
        public long FailureCount { get; set; }
        public string PassesEncounteringFailure { get; set; }
    }
}
