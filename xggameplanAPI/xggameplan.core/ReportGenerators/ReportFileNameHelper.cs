using System;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;

namespace xggameplan.core.ReportGenerators
{
    /// <summary>
    /// Contains a bunch of methods to generate predefined report file paths for S3 bucket.
    /// </summary>
    public static class ReportFileNameHelper
    {
        /// <summary>
        /// Returns file path for scenario campaign result report file.
        /// </summary>
        public static string ScenarioCampaignResult(Scenario scenario, DateTime runStartDateTime)
        {
            if (scenario is null)
            {
                throw new ArgumentNullException(nameof(scenario));
            }

            var fileName =
                $"{scenario.Name.Substring(0, Math.Min(scenario.Name.Length, 20))}_{runStartDateTime:yyyyMMdd}_CampaignData_{scenario.Id.ToString()}.xlsx";

            return $"reports/{scenario.Id.ToString()}/scenario-campaign-results/{fileName}";
        }

        /// <summary>
        /// Returns file path for recommendation result report file.
        /// </summary>
        public static string RecommendationResult(Run run, Scenario scenario)
        {
            var fileName =
                $"{run.Description.Substring(0, Math.Min(run.Description.Length, 40))}_{run.ExecuteStartedDateTime.Value.ToString("yyyyMMddHHmmss")}_{scenario.Name.Substring(0, Math.Min(scenario.Name.Length, 40))}.xlsx";
            return $"reports/{scenario.Id.ToString()}/recommendations/{fileName}";
        }
    }
}
