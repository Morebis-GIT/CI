using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using FluentAssertions;
using FluentAssertions.Execution;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Metadatas;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Interfaces;
using xggameplan.core.Services;
using xggameplan.Reports.Common;
using xggameplan.Reports.ExcelReports.Campaigns;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Campaigns")]
    public class CampaignsControllerDeleteByIdShould : CampaignsControllerTestBase
    {
        public Mock<IPassRepository> PassRepository { get; private set; }

        [SetUp]
        public void SetUp()
        {
            AssumeDependenciesAreSupplied();
            AssumeTargetIsInitialised();
        }

        [Test]
        public async Task ReturnNotFoundResultWhenNoRecordIsFoundForTheSuppliedId()
        {
            var campaignId = AssumeValidIdIsSupplied();
            AssumeCampaignRepositoryGetReturnsNoRecord();

            var result = AssumeDeleteByIdIsRequested(campaignId);

            _ = result.Should().BeAssignableTo<NotFoundResult>(
                  "CampaignsController Delete By Id Failed to return NotFoundResult");
        }

        [Test]
        public async Task ReturnBadRequestResultWhenAnEmptyIdIsSupplied()
        {
            var campaignId = AssumeAnEmptyIdIsSupplied();

            var result = AssumeDeleteByIdIsRequested(campaignId);

            _ = result.Should().BeAssignableTo<BadRequestErrorMessageResult>(
                   "CampaignsController Delete By Id Failed to return BadRequestErrorMessageResult");
        }

        [Test]
        public async Task ReturnNoContentResultWhenACampaignIsFoundForTheSuppliedIdAndSuccessfullyDeleted()
        {
            var campaignId = AssumeValidIdIsSupplied();
            var campaign = CreateValidCampaign(campaignId, CreateCampaignExternalIds(1).FirstOrDefault());
            AssumeCampaignRepositoryGetReturnsAMatchingCampaign(campaign);
            AssumeCampaignRepositoryGetWithProduct(campaign);

            var result = AssumeDeleteByIdIsRequested(campaignId) as StatusCodeResult;

            using (new AssertionScope())
            {
                _ = result.StatusCode.Should().Be(HttpStatusCode.NoContent,
                                  "CampaignsController Delete By Id Failed to return NoContent result");
                CampaignRepository.Verify(a => a.Get(It.IsAny<Guid>()), Times.Once(),
                                   "Failed to use CampaignRepository to Get the matching campaign for deletion");
                CampaignCleaner.Verify(a => a.ExecuteAsync(It.IsAny<Guid>(), default), Times.Once(),
                                   "Failed to use CampaignRepository to delete the campaign");
            }
        }

        [Test]
        public async Task ReturnNoContentResultWhenSuccessfullyDeletedAndUpdatedDefaultScenarioByRemovingCampaignPassPriorityOfDeletedCampaign()
        {
            var campaignId = AssumeValidIdIsSupplied();
            var campaign = CreateValidCampaign(campaignId, CreateCampaignExternalIds(1).FirstOrDefault());
            AssumeCampaignRepositoryGetReturnsAMatchingCampaign(campaign);
            var tenantSettings = AssumeTenantSettingsExistsAndContainsDefaultScenarioId();
            AssumeTenantSettingsRepositoryGetDefaultScenarioIdReturns(tenantSettings.DefaultScenarioId);
            var defaultScenario = CreateValidScenario(tenantSettings.DefaultScenarioId, 6, new List<string> { campaign.ExternalId });
            AssumeScenarioRepositoryGetReturnsAScenario(defaultScenario);
            AssumeCampaignRepositoryGetWithProduct(campaign);

            var result = AssumeDeleteByIdIsRequested(campaignId) as StatusCodeResult;

            using (new AssertionScope())
            {
                _ = result.StatusCode.Should().Be(HttpStatusCode.NoContent,
                                  "CampaignsController Delete By Id Failed to return NoContent result");
                CampaignRepository.Verify(a => a.Get(It.IsAny<Guid>()), Times.Once(),
                                   "Failed to use CampaignRepository to Get the matching Campaign for deletion");
                CampaignCleaner.Verify(a => a.ExecuteAsync(It.IsAny<Guid>(), default), Times.Once(),
                                   "Failed to use CampaignRepository to delete the campaign");
            }
        }

        [Test]
        public async Task ReturnNoContentResultWhenSuccessfullyDeletedAndNotUpdatedCampaignPassPriorityOfDeletedCampaignIfNoDefaultScenarioExist()
        {
            var campaignId = AssumeValidIdIsSupplied();
            var campaign = CreateValidCampaign(campaignId, CreateCampaignExternalIds(1).FirstOrDefault());
            AssumeCampaignRepositoryGetReturnsAMatchingCampaign(campaign);
            var tenantSettings = AssumeTenantSettingsExistsAndDoNotContainDefaultScenarioId();
            AssumeTenantSettingsRepositoryGetDefaultScenarioIdReturns(tenantSettings.DefaultScenarioId);
            AssumeCampaignRepositoryGetWithProduct(campaign);

            var result = AssumeDeleteByIdIsRequested(campaignId) as StatusCodeResult;

            using (new AssertionScope())
            {
                _ = result.StatusCode.Should().Be(HttpStatusCode.NoContent,
                                  "CampaignsController Delete By Id Failed to return NoContent result");
                CampaignRepository.Verify(a => a.Get(It.IsAny<Guid>()), Times.Once(),
                                   "Failed to use CampaignRepository to Get the matching Campaign for deletion");
                CampaignCleaner.Verify(a => a.ExecuteAsync(It.IsAny<Guid>(), default), Times.Once(),
                                   "Failed to use CampaignRepository to delete the campaign");
            }
        }

        private IHttpActionResult AssumeDeleteByIdIsRequested(Guid withCampaignId)
        {
            return Target.DeleteAsync(withCampaignId).GetAwaiter().GetResult();
        }

        private Guid AssumeValidIdIsSupplied()
        {
            return Guid.NewGuid();
        }

        private Guid AssumeAnEmptyIdIsSupplied()
        {
            return Guid.Empty;
        }

        private void AssumeDependenciesAreSupplied()
        {
            CampaignRepository = new Mock<ICampaignRepository>();
            TenantSettingsRepository = new Mock<ITenantSettingsRepository>();
            ScenarioRepository = new Mock<IScenarioRepository>();
            MetadataRepository = new Mock<IMetadataRepository>();
            ProgrammeCategoryRepository = new Mock<IProgrammeCategoryHierarchyRepository>();
            DemographicRepository = new Mock<IDemographicRepository>();
            SalesAreaRepository = new Mock<ISalesAreaRepository>();
            ProductRepository = new Mock<IProductRepository>();
            ReportColumnFormatter = new Mock<IReportColumnFormatter>();
            CampaignExcelReportGenerator = new Mock<ICampaignExcelReportGenerator>();
            PassRepository = new Mock<IPassRepository>();
            FeatureManager = new Mock<IFeatureManager>();
            CampaignCleaner = new Mock<ICampaignCleaner>();
        }

        private void AssumeTargetIsInitialised()
        {
            var flattener = new CampaignFlattener(ProductRepository.Object, DemographicRepository.Object,
                null, Mapper);
            var campaignPassPrioritiesService = new CampaignPassPrioritiesService(
                CampaignRepository.Object,
                Mapper,
                PassRepository.Object,
                ScenarioRepository.Object);

            Target = new CampaignsController(CampaignRepository.Object, null, null,
                Mapper, DemographicRepository.Object,
                SalesAreaRepository.Object, ProductRepository.Object,
                CampaignExcelReportGenerator.Object, ReportColumnFormatter.Object,
                null, null, ProgrammeCategoryRepository.Object, FeatureManager.Object,
                flattener, CampaignCleaner.Object, campaignPassPrioritiesService);
        }

        [TearDown]
        public void TearDown()
        {
            CampaignRepository = null;
            TenantSettingsRepository = null;
            ScenarioRepository = null;
            Target?.Dispose();
            Target = null;
        }
    }
}
