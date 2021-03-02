using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class ScenarioCampaignFailures_BySearch
        : AbstractIndexCreationTask<ScenarioCampaignFailure,
            ScenarioCampaignFailures_BySearch.IndexedFields>
    {
        public static string DefaultIndexName => "ScenarioCampaignFailures/BySearch";

        public class IndexedFields
        {
            public Guid ScenarioId { get; set; }
            public string SalesAreaGroup { get; set; }
            public string SalesArea { get; set; }
            public string ExternalCampaignId { get; set; }
            public DateTime StrikeWeightStartDate { get; set; }
            public DateTime StrikeWeightEndDate { get; set; }
            public int MultipartNo { get; set; }
            public TimeSpan DayPartStartTime { get; set; }
            public TimeSpan DayPartEndTime { get; set; }
            public string DayPartDays { get; set; }
            public int FailureType { get; set; }
            public long FailureCount { get; set; }
        }

        public ScenarioCampaignFailures_BySearch()
        {
            Map = scenariocampaignfailures =>
                from scf in scenariocampaignfailures
                select new
                {
                    scf.ScenarioId,
                    scf.SalesAreaGroup,
                    scf.SalesArea,
                    scf.ExternalCampaignId,
                    scf.StrikeWeightStartDate,
                    scf.StrikeWeightEndDate,
                    scf.MultipartNo,
                    scf.DayPartStartTime,
                    scf.DayPartEndTime,
                    scf.DayPartDays,
                    scf.FailureType,
                    scf.FailureCount,
                    scf.PassesEncounteringFailure
                };

            Index(p => p.SalesAreaGroup, FieldIndexing.Analyzed);
            Index(p => p.ScenarioId, FieldIndexing.Analyzed);
            Index(p => p.ExternalCampaignId, FieldIndexing.Analyzed);
        }
    }
}
