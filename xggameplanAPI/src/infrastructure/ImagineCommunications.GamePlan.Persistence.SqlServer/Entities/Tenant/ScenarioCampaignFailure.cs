using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant
{
    public class ScenarioCampaignFailure : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid ScenarioId { get; set; }
        public string ExternalCampaignId { get; set; }
        public string SalesAreaGroup { get; set; }
        public string SalesArea { get; set; }
        public TimeSpan Length { get; set; }
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
