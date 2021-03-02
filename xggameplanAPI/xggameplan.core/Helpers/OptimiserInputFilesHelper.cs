using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using xggameplan.AutoGen.AgDataPopulation;

namespace xggameplan.core.Helpers
{
    /// <summary>
    /// Methods used to help process optimiser input file data
    /// </summary>
    public static class OptimiserInputFilesHelper
    {
        /// <summary>
        /// Filter a list of campaigns based on shared excluded campaign ids in run scenarios
        /// </summary>
        /// <param name="scenarioRepository">Instance of the Scenario Repository</param>
        /// <param name="run">Run for generating optimiser input files</param>
        /// <param name="campaigns">List of all campaign records in the database</param>
        /// <returns></returns>
        public static List<Campaign> FilterOutExcludedCampaigns(
            IScenarioRepository scenarioRepository,
            Run run,
            IEnumerable<Campaign> campaigns)
        {
            List<int> filteredCampaignIds = new List<int>();
            foreach (var runScenario in run.Scenarios)
            {
                var scenario = scenarioRepository.Get(runScenario.Id);
                if (scenario is null)
                {
                    continue;
                }
                var campaignsFilteredByPriority = scenario.ToAgScenarioCampaignPass();
                if (campaignsFilteredByPriority != null && campaignsFilteredByPriority.AgScenarioCampaignPasses.Any())
                {
                    filteredCampaignIds.AddRange(
                        campaignsFilteredByPriority
                        .AgScenarioCampaignPasses
                        .Select(o => o.CampaignCustomNo)
                        .ToList()
                    );
                }

                ReturnCampaignsThatShouldNotBeFiltered(run, scenario, filteredCampaignIds);
            }
            filteredCampaignIds = filteredCampaignIds.Distinct().ToList();

            return campaigns.Where(c => filteredCampaignIds.Any(o => o == c.CustomId)).ToList();
        }

        /// <summary>
        /// Re-adds campaign ids if they were marked as IncludeRightsizer or ISR
        /// </summary>
        /// <param name="run">Original Run instance</param>
        /// <param name="scenario">Scenario with the campaigns</param>
        /// <param name="filteredCampaignIds">A list of already filtered campaign ids</param>
        private static void ReturnCampaignsThatShouldNotBeFiltered(Run run, Scenario scenario, List<int> filteredCampaignIds)
        {
            foreach (var cpp in scenario.CampaignPassPriorities)
            {
                if ((run.RightSizer && cpp.Campaign.IncludeRightSizer != IncludeRightSizer.No)
                    || (run.ISR && cpp.Campaign.InefficientSpotRemoval))
                {
                    filteredCampaignIds.Add(cpp.Campaign.CustomId);
                }

                if (!cpp.Campaign.IncludeOptimisation &&
                    filteredCampaignIds.Any(x => x == cpp.Campaign.CustomId) &&
                    !(
                        (run.ISR && cpp.Campaign.InefficientSpotRemoval) ||
                        (run.RightSizer && cpp.Campaign.IncludeRightSizer != IncludeRightSizer.No)
                    ))
                {
                    _ = filteredCampaignIds.RemoveAll(fc => fc == cpp.Campaign.CustomId);
                }
            }
        }
    }
}
