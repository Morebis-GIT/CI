using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using xggameplan.core.Interfaces;

namespace xggameplan.core.Services
{
    public class CampaignPassPrioritiesService : ICampaignPassPrioritiesService
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IMapper _mapper;
        private readonly IPassRepository _passRepository;
        private readonly IScenarioRepository _scenarioRepository;

        public CampaignPassPrioritiesService(
            ICampaignRepository campaignRepository,
            IMapper mapper,
            IPassRepository passRepository,
            IScenarioRepository scenarioRepository
            )
        {
            _campaignRepository = campaignRepository;
            _mapper = mapper;
            _passRepository = passRepository;
            _scenarioRepository = scenarioRepository;
        }

        public void AddNewCampaignPassPrioritiesToAllScenariosInLibrary()
        {
            GetAllScenariosAndCampaigns(
                out IEnumerable<Scenario> scenarios,
                out IList<CampaignWithProductFlatModel> usingAllCampaigns);

            Dictionary<string, CampaignWithProductFlatModel> allCampaigns =
                usingAllCampaigns?.ToDictionary(x => x.ExternalId, x => x);

            foreach (Scenario scenario in scenarios)
            {
                List<Pass> forScenarioPasses = _passRepository
                    .FindByIds(scenario.Passes.Select(p => p.Id))
                    .ToList();

                CampaignPassPrioritiesServiceMapper.AmendCampaignPassPrioritiesForNewCampaigns(
                    scenario,
                    forScenarioPasses,
                    allCampaigns,
                    _passRepository,
                    _mapper);

                _scenarioRepository.Update(scenario);
            }

            _scenarioRepository.SaveChanges();
        }

        private void GetAllScenariosAndCampaigns(
            out IEnumerable<Scenario> scenarios,
            out IList<CampaignWithProductFlatModel> allCampaigns)
        {
            scenarios = _scenarioRepository.GetLibraried();

            // Get all the campaigns with product info.
            // This will be used to add CampaignPassPriorities for new Campaigns
            // to each Scenario
            var campaignsResult = _campaignRepository.GetWithProduct(null);
            allCampaigns = (campaignsResult.Items != null && campaignsResult.Items.Count > 0) ?
                campaignsResult.Items.ToList() :
                null;
        }
    }
}
