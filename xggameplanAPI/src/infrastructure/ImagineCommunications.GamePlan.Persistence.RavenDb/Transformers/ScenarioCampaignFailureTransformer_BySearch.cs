using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using NodaTime;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Transformers
{
    public class ScenarioCampaignFailureTransformer_BySearch : AbstractTransformerCreationTask<ScenarioCampaignFailure>
    {
        public class Result
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

        public ScenarioCampaignFailureTransformer_BySearch()
        {
            TransformResults = scenariocampaignfailuremodels =>
                from scf in scenariocampaignfailuremodels
                let scfId = scf.Id.ToString()
                select new
                {
                    Id = int.Parse(scfId.Substring(scfId.IndexOf('/') + 1)),
                    scf.ScenarioId,
                    scf.ExternalCampaignId,
                    scf.SalesAreaGroup,
                    scf.SalesArea,
                    scf.Length,
                    scf.MultipartNo,
                    scf.StrikeWeightStartDate,
                    scf.StrikeWeightEndDate,
                    scf.DayPartStartTime,
                    scf.DayPartEndTime,
                    scf.DayPartDays,
                    scf.FailureType,
                    scf.FailureCount,
                    scf.PassesEncounteringFailure
                };
        }
    }
}
