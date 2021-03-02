using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using xggameplan.Model;

namespace xggameplan.core.Services
{
    public static class CampaignPassPrioritiesServiceMapper
    {
        public static void AmendCampaignPassPriorities(
            List<Scenario> inScenarios,
            List<Pass> forAllScenariosPasses,
            List<CampaignWithProductFlatModel> usingAllCampaigns,
            IPassRepository passRepository,
            IMapper mapper)
        {
            if (inScenarios?.Any() != true || usingAllCampaigns?.Any() != true)
            {
                return;
            }

            var allCampaigns = usingAllCampaigns.ToDictionary(x => x.ExternalId, x => x);
            var campaignExternalIds = allCampaigns.Keys.ToList();

            foreach (var scenario in inScenarios)
            {
                AmendCampaignPassPrioritiesOfDeletedCampaigns(scenario, campaignExternalIds);
                AmendCampaignPassPrioritiesForNewCampaigns(
                    scenario,
                    forAllScenariosPasses,
                    allCampaigns,
                    passRepository,
                    mapper);
            }
        }

        public static void AmendCampaignPassPrioritiesForNewCampaigns(
            Scenario inScenario,
            List<Pass> forAllScenariosPasses,
            Dictionary<string, CampaignWithProductFlatModel> usingAllCampaigns,
            IPassRepository passRepository,
            IMapper mapper)
        {
            if (
                inScenario?.IsLibraried == null ||
                !inScenario.IsLibraried.Value ||
                usingAllCampaigns?.Any() != true
                )
            {
                return;
            }

            // Get all campaign external ids from existing campaign pass
            // priorities in the scenario
            HashSet<string> existingCppCampaignExternalIds = null;

            if (inScenario.CampaignPassPriorities != null)
            {
                existingCppCampaignExternalIds = new HashSet<string>();

                foreach (var campaignPassPriority in inScenario.CampaignPassPriorities)
                {
                    if (campaignPassPriority?.Campaign == null)
                    {
                        continue;
                    }

                    existingCppCampaignExternalIds.Add(campaignPassPriority.Campaign.ExternalId);

                    if (!usingAllCampaigns.ContainsKey(campaignPassPriority.Campaign.ExternalId))
                    {
                        continue;
                    }

                    var campaign =
                        mapper.Map<CompactCampaign>(usingAllCampaigns[campaignPassPriority.Campaign.ExternalId]);

                    if (campaign != null && !campaign.Equals(campaignPassPriority.Campaign))
                    {
                        campaignPassPriority.Campaign = campaign;
                    }
                }
            }

            // Get the pass details for the existing Scenario passes
            var existingScenarioPassIds = inScenario.Passes.Select(p => p.Id).ToList();
            var existingScenarioPasses = forAllScenariosPasses?.Any() == true ?
                forAllScenariosPasses.Where(p => existingScenarioPassIds.Contains(p.Id)).ToList() :
                passRepository?.FindByIds(existingScenarioPassIds).ToList();

            var existingCampaigns = usingAllCampaigns.Values.ToList();

            // Create CampaignPassPriorities for the new campaigns which are not
            // in the campaignPassPriorities
            var campaignPassPrioritiesForNewCampaigns =
                CreateCampaignPassPriorities(
                    existingCampaigns,
                    existingCppCampaignExternalIds,
                    existingScenarioPasses, mapper);

            if (campaignPassPrioritiesForNewCampaigns?.Any() == true)
            {
                // Apply new Campaign Pass Priorities to the Scenario
                var newCppList =
                    MapToCampaignPassPriorities(
                        campaignPassPrioritiesForNewCampaigns,
                        existingCampaigns,
                        mapper);
                inScenario.CampaignPassPriorities ??= new List<CampaignPassPriority>();
                inScenario.CampaignPassPriorities.AddRange(newCppList);
            }
        }

        public static void AmendCampaignPassPrioritiesOfDeletedCampaigns(
            Scenario inScenario,
            List<string> usingAllCampaignsExternalIds)
        {
            if (inScenario?.CampaignPassPriorities?.Any() != true)
            {
                return;
            }

            // Get the list of deleted Campaigns ExternalIds i.e. the ExternalIds
            // that are not in the current list of Campaigns
            var deletedCampaignsExternalIds = new HashSet<string>(
                inScenario
                .CampaignPassPriorities
                .Select(c => c.Campaign.ExternalId));

            if (usingAllCampaignsExternalIds?.Any() == true)
            {
                deletedCampaignsExternalIds.ExceptWith(usingAllCampaignsExternalIds);
            }

            if (deletedCampaignsExternalIds.Count > 0)
            {
                inScenario.CampaignPassPriorities = inScenario
                    .CampaignPassPriorities
                    .Where(c => !deletedCampaignsExternalIds.Contains(c.Campaign.ExternalId))
                    .ToList();
            }
        }

        public static List<CampaignPassPriority> MapToCampaignPassPriorities(
            List<CreateCampaignPassPriorityModel> campaignPassPriorities,
            List<CampaignWithProductFlatModel> allCampaigns,
            IMapper mapper)
        {
            if (campaignPassPriorities == null ||
                allCampaigns == null)
            {
                return null;
            }

            return mapper.Map<List<CampaignPassPriority>>(
                Tuple.Create(campaignPassPriorities, allCampaigns));
        }

        public static List<CreateCampaignPassPriorityModel> CreateCampaignPassPriorityModels(
            List<CampaignWithProductFlatModel> forCampaigns,
            List<PassModel> withPasses,
            IMapper mapper)
        {
            return mapper.Map<List<CreateCampaignPassPriorityModel>>(
                Tuple.Create(forCampaigns, withPasses));
        }

        public static List<CreateCampaignPassPriorityModel> CreateCampaignPassPriorityModels(
            List<CampaignWithProductFlatModel> forCampaigns,
            List<Pass> withPasses,
            IMapper mapper)
        {
            return mapper.Map<List<CreateCampaignPassPriorityModel>>(
                Tuple.Create(forCampaigns, withPasses));
        }

        private static List<CreateCampaignPassPriorityModel> CreateCampaignPassPriorities(
            List<CampaignWithProductFlatModel> usingAllCampaigns,
            HashSet<string> existingCppCampaignExternalIds,
            List<Pass> forScenarioPasses,
            IMapper mapper)
        {
            if (forScenarioPasses == null || forScenarioPasses.Count == 0)
            {
                return null;
            }

            // Get the new campaigns which are not in the default scenario's CampaignPassPriorities
            var campaignsWithoutPassPriorities = existingCppCampaignExternalIds is null
                ? usingAllCampaigns
                : usingAllCampaigns?.Where(c => !existingCppCampaignExternalIds.Contains(c.ExternalId)).ToList();

            List<CreateCampaignPassPriorityModel> campaignPassPrioritiesForNewCampaigns = null;

            // Create CampaignPassPriorities with new campaigns for passes in the scenario
            if (campaignsWithoutPassPriorities != null && campaignsWithoutPassPriorities.Count > 0)
            {
                campaignPassPrioritiesForNewCampaigns =
                    ProduceCreateCampaignPassPriorityModels(
                        campaignsWithoutPassPriorities,
                        forScenarioPasses,
                        mapper);
            }

            return campaignPassPrioritiesForNewCampaigns;
        }

        private static List<CreateCampaignPassPriorityModel> ProduceCreateCampaignPassPriorityModels(
            List<CampaignWithProductFlatModel> forCampaigns,
            List<Pass> withPasses,
            IMapper mapper)
        {
            List<CreateCampaignPassPriorityModel> result = null;

            if (forCampaigns != null &&
                forCampaigns.Count > 0 &&
                withPasses != null &&
                withPasses.Count > 0)
            {
                result = CreateCampaignPassPriorityModels(
                    forCampaigns,
                    withPasses,
                    mapper);
            }

            return result;
        }
    }
}
