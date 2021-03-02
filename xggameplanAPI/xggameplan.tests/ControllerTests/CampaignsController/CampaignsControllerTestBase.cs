using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Interfaces;
using xggameplan.Reports.Common;
using xggameplan.Reports.ExcelReports.Campaigns;

namespace xggameplan.tests.ControllerTests
{
    public abstract class CampaignsControllerTestBase
    {
        protected Fixture Fixture { get; private set; }
        protected IMapper Mapper { get; private set; }
        protected CampaignsController Target { get; set; }
        protected Mock<ITenantSettingsRepository> TenantSettingsRepository { get; set; }
        protected Mock<ICampaignRepository> CampaignRepository { get; set; }
        protected Mock<IScenarioRepository> ScenarioRepository { get; set; }
        protected Mock<IMetadataRepository> MetadataRepository { get; set; }
        protected Mock<IDemographicRepository> DemographicRepository { get; set; }
        protected Mock<ISalesAreaRepository> SalesAreaRepository { get; set; }
        protected Mock<IProductRepository> ProductRepository { get; set; }
        protected Mock<ICampaignExcelReportGenerator> CampaignExcelReportGenerator { get; set; }
        protected Mock<IReportColumnFormatter> ReportColumnFormatter { get; set; }
        protected Mock<IProgrammeCategoryHierarchyRepository> ProgrammeCategoryRepository { get; set; }
        protected Mock<IFeatureManager> FeatureManager { get; set; }
        protected Mock<ICampaignCleaner> CampaignCleaner { get; set; }

        [OneTimeSetUp]
        public void OnInit()
        {
            Fixture = new Fixture();

            Mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);
        }

        protected List<string> CreateCampaignExternalIds(int noOfExternalIds)
        {
            return Fixture.CreateMany<string>(noOfExternalIds).ToList();
        }

        protected TenantSettings AssumeTenantSettingsExistsAndContainsDefaultScenarioId()
        {
            return CreateTenantSettings(Guid.NewGuid());
        }

        protected TenantSettings AssumeTenantSettingsExistsAndDoNotContainDefaultScenarioId()
        {
            return CreateTenantSettings(Guid.Empty);
        }

        protected TenantSettings CreateTenantSettings(Guid withDefaultScenarioId)
        {
            return Fixture.Build<TenantSettings>()
                           .With(a => a.DefaultScenarioId, withDefaultScenarioId)
                           .Create();
        }

        protected void AssumeCampaignRepositoryGetReturnsNoRecord()
        {
            SetupCampaignRepositoryGetToReturn(null);
        }

        protected void AssumeCampaignRepositoryGetReturnsAMatchingCampaign(Campaign campaign)
        {
            SetupCampaignRepositoryGetToReturn(campaign);
        }

        protected void AssumeCampaignRepositoryGetAllActiveExternalIdsReturnsMatchingCampaignExternalIds(IEnumerable<string> campaignExternalIds)
        {
            SetupCampaignRepositoryGetAllActiveExternalIdsToReturn(campaignExternalIds);
        }

        protected void AssumeCampaignRepositoryGetAllActiveExternalIdsReturnsNoMatchingRecord()
        {
            SetupCampaignRepositoryGetAllActiveExternalIdsToReturn(Array.Empty<string>());
        }

        protected void AssumeTenantSettingsRepositoryGetDefaultScenarioIdReturns(Guid defaultScenarioId)
        {
            SetupTenantSettingsRepositoryGetDefaultScenarioIdToReturn(defaultScenarioId);
        }

        protected void AssumeScenarioRepositoryGetReturnsAScenario(Scenario scenario)
        {
            SetupScenarioRepositoryGetToReturn(scenario);
        }

        protected void AssumeCampaignRepositoryGetWithProduct(Campaign campaign = null)
        {
            SetupCampaignRepositoryGetWithProduct(campaign);
        }

        protected void SetupTenantSettingsRepositoryGetDefaultScenarioIdToReturn(Guid scenarioId)
        {
            _ = TenantSettingsRepository.Setup(a => a.GetDefaultScenarioId()).Returns(scenarioId);
        }

        protected void SetupScenarioRepositoryGetToReturn(Scenario scenario)
        {
            _ = ScenarioRepository.Setup(a => a.Get(It.IsAny<Guid>())).Returns(scenario);
        }

        protected void SetupCampaignRepositoryGetToReturn(Campaign campaign)
        {
            _ = CampaignRepository.Setup(a => a.Get(It.IsAny<Guid>())).Returns(campaign);
        }

        protected void SetupCampaignRepositoryGetAllActiveExternalIdsToReturn(IEnumerable<string> campaignExternalIds)
        {
            _ = CampaignRepository.Setup(a => a.GetAllActiveExternalIds()).Returns(campaignExternalIds);
        }

        protected void SetupCampaignRepositoryGetWithProduct(Campaign campaign)
        {
            if (campaign == null)
            {
                campaign = new Campaign();
            }

            _ = CampaignRepository.Setup(c =>
              c.GetWithProduct(null))
                .Returns(new PagedQueryResult<CampaignWithProductFlatModel>(1, new List<CampaignWithProductFlatModel>()
                {
                    Mapper.Map<CampaignWithProductFlatModel>(campaign)
                }));
        }

        protected Campaign CreateValidCampaign(Guid id, string externalId = null)
        {
            id = id != Guid.Empty ? id : Guid.NewGuid();
            externalId = !string.IsNullOrWhiteSpace(externalId) ? externalId : CreateCampaignExternalIds(1).FirstOrDefault();

            return Fixture.Build<Campaign>()
                           .With(a => a.Id, id)
                           .With(a => a.ExternalId, externalId)
                           .Create();
        }

        protected Scenario CreateValidScenario(Guid usingId, int noOfCampaignPassPriorities,
                                             List<string> includingCampaignPasspriorityWithCampaignExternalIds)
        {
            return Fixture.Build<Scenario>()
                           .With(a => a.Id, usingId)
                           .With(a => a.CampaignPassPriorities, CreateCampaignPassPriority(noOfCampaignPassPriorities,
                                                                includingCampaignPasspriorityWithCampaignExternalIds))
                           .Create();
        }

        protected List<CampaignPassPriority> CreateCampaignPassPriority(int noOfCampaignPassPriorities,
                                                                      List<string> includingCampaignExternalIds)
        {
            var campaignPassPriorities = Fixture.CreateMany<CampaignPassPriority>(noOfCampaignPassPriorities).ToList();
            if (includingCampaignExternalIds.Count <= noOfCampaignPassPriorities)
            {
                for (int i = 0; i < includingCampaignExternalIds.Count; i++)
                {
                    campaignPassPriorities[i].Campaign.ExternalId = includingCampaignExternalIds[i];
                }
            }

            return campaignPassPriorities;
        }

        [OneTimeTearDown]
        public void OnDestroy()
        {
            Fixture = null;
            Mapper = null;
        }
    }
}
