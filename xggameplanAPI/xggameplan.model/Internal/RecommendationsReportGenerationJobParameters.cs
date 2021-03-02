using System;

namespace xggameplan.model.Internal
{
    public class RecommendationsReportGenerationJobParameters
    {
        public Guid ScenarioId { get; set; }

        public string FilePath { get; set; }

        public string ReportReference { get; set; }
    }
}
