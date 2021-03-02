using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories;
using Raven.Client;

namespace xggameplan.Updates
{
    internal class UpdateStepXGGT9170_SyncScenarioCampaignsWihActualCampaigns : PatchUpdateStepBase, IUpdateStep
    {
        private readonly IEnumerable<string> _tenantConnectionStrings;
        private readonly string _updatesFolder;
        private readonly string _rollBackFolder;

        public UpdateStepXGGT9170_SyncScenarioCampaignsWihActualCampaigns(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            ValidateParametersBeforeUse(tenantConnectionStrings, updatesFolder);

            _tenantConnectionStrings = tenantConnectionStrings;
            _updatesFolder = updatesFolder;
            _rollBackFolder = Path.Combine(_updatesFolder, "RollBack");
            _ = Directory.CreateDirectory(_rollBackFolder);
        }

        public Guid Id => new Guid("45789645-7A93-4EEF-BD11-02E8171326BC");

        public void Apply()
        {
            foreach (string tenantConnectionString in _tenantConnectionStrings)
            {
                using (IDocumentStore documentStore = DocumentStoreFactory.CreateStore(tenantConnectionString))
                using (IDocumentSession session = documentStore.OpenSession())
                {
                    var scenarioRepository = new RavenScenarioRepository(session);
                    var campaignRepository = new RavenCampaignRepository(session, null, null);

                    IEnumerable<Scenario> _scenarios;
                    List<CampaignWithProductFlatModel> allCampaigns;
                    GetAllScenariosAndCampaigns(scenarioRepository, campaignRepository, out _scenarios, out allCampaigns);

                    foreach (Scenario scenario in _scenarios)
                    {
                        AmendCampaignPassPrioritiesForNewCampaigns(scenario, allCampaigns);
                        scenarioRepository.Update(scenario);
                    }
                    scenarioRepository.SaveChanges();
                }
            }
        }

        private void AmendCampaignPassPrioritiesForNewCampaigns(Scenario inScenario, List<CampaignWithProductFlatModel> usingAllCampaigns)
        {
            if (inScenario != null && usingAllCampaigns?.Any() == true)
            {
                if (inScenario.CampaignPassPriorities != null)
                {
                    inScenario.CampaignPassPriorities.ForEach(campaignPassPriorities =>
                    {
                        if (campaignPassPriorities != null && campaignPassPriorities.Campaign != null)
                        {
                            if (usingAllCampaigns.Any(campaign => campaignPassPriorities.Campaign.ExternalId == campaign.ExternalId))
                            {
                                var campaignModel = usingAllCampaigns.Find(c => campaignPassPriorities.Campaign.ExternalId == c.ExternalId);
                                var campaign = CreateCompactCampaign(campaignModel);
                                if (campaign != null && !campaign.Equals(campaignPassPriorities.Campaign))
                                {
                                    campaignPassPriorities.Campaign = campaign;
                                }
                            }
                        }
                    });
                }
            }
        }

        private CompactCampaign CreateCompactCampaign(CampaignWithProductFlatModel campaignModel)
        {
            var model = new CompactCampaign();
            model.Uid = campaignModel.Uid;
            model.CustomId = campaignModel.CustomId;
            model.Status = campaignModel.Status;
            model.Name = campaignModel.Name;
            model.ExternalId = campaignModel.ExternalId;
            model.CampaignGroup = campaignModel.CampaignGroup;
            model.StartDateTime = campaignModel.StartDateTime;
            model.EndDateTime = campaignModel.EndDateTime;
            model.ProductExternalRef = campaignModel.ProductExternalRef;
            model.ProductName = campaignModel.ProductName;
            model.AdvertiserName = campaignModel.AdvertiserName;
            model.AgencyName = campaignModel.AgencyName;
            model.BusinessType = campaignModel.BusinessType;
            model.Demographic = campaignModel.Demographic;
            model.RevenueBudget = campaignModel.RevenueBudget;
            model.TargetRatings = campaignModel.TargetRatings;
            model.ActualRatings = campaignModel.ActualRatings;
            model.IsPercentage = campaignModel.IsPercentage;
            model.IncludeOptimisation = campaignModel.IncludeOptimisation;
            model.InefficientSpotRemoval = campaignModel.InefficientSpotRemoval;
            model.IncludeRightSizer = campaignModel.IncludeRightSizer;
            model.DefaultCampaignPassPriority = campaignModel.DefaultCampaignPassPriority;
            model.ClashCode = campaignModel.ClashCode;
            model.ClashDescription = campaignModel.ClashDescription;
            return model;
        }

        private void GetAllScenariosAndCampaigns(RavenScenarioRepository scenarioRepository, RavenCampaignRepository campaignRepository, out IEnumerable<Scenario> _scenarios, out List<CampaignWithProductFlatModel> allCampaigns)
        {
            _scenarios = GetAllScenariosInLibrary(scenarioRepository);
            //get all the campaigns with product info. this will be used to add CampaignPassPriorities for new Campaigns to each Scenario
            var campaignsResult = campaignRepository.GetWithProduct(null);
            allCampaigns = campaignsResult.Items != null && campaignsResult.Items.Any() ? campaignsResult.Items.ToList() : null;
        }

        private IEnumerable<Scenario> GetAllScenariosInLibrary(RavenScenarioRepository scenarioRepository)
        {
            return scenarioRepository.GetLibraried();
        }

        public void RollBack()
        {
            throw new NotImplementedException();
        }

        public int Sequence => 1;

        public string Name => "XGGP-9170 : Sync Scenario Campaigns Wih Actual Campaigns";

        public bool SupportsRollback => false;

        private static void ValidateParametersBeforeUse(IEnumerable<string> tenantConnectionStrings, string updatesFolder)
        {
            _ = UpdateValidator.ValidateTenantConnectionString(tenantConnectionStrings, true);
            _ = UpdateValidator.ValidateUpdateFolderPath(updatesFolder, true);
        }
    }
}
