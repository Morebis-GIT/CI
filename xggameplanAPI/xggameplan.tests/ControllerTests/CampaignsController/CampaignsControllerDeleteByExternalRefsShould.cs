using System.Collections.Generic;
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
    public class CampaignsControllerDeleteByExternalRefsShould : CampaignsControllerTestBase
    {
        public Mock<IPassRepository> PassRepository { get; private set; }

        [SetUp]
        public void SetUp()
        {
            AssumeDependenciesAreSupplied();
            AssumeTargetIsInitialised();
        }

        [Test]
        public async Task ReturnNotFoundResultWhenNoRecordsAreFoundForTheSuppliedExternalRefs()
        {
            var externalRefs = AssumeValidExternalRefsAreSupplied(6);
            AssumeCampaignRepositoryGetAllActiveExternalIdsReturnsNoMatchingRecord();

            var result = AssumeDeleteByExternalRefsIsRequested(externalRefs);

            _ = result.Should().BeAssignableTo<NotFoundResult>(
                   "CampaignsController Delete By ExternalRefs Failed to return NotFoundResult");
        }

        [Test]
        public async Task ReturnBadRequestResultWhenNoExternalRefsAreSupplied()
        {
            var externalRefs = AssumeNoExternalRefsAreSupplied();

            var result = AssumeDeleteByExternalRefsIsRequested(externalRefs);

            _ = result.Should().BeAssignableTo<BadRequestErrorMessageResult>(
                   "CampaignsController Delete By ExternalRefs Failed to return BadRequestErrorMessageResult");
        }

        [Test]
        public async Task ReturnNoContentResultWhenCampaignsAreFoundForTheSuppliedExternalRefsAndSuccessfullyDeleted()
        {
            var externalRefs = AssumeValidExternalRefsAreSupplied(6);
            AssumeCampaignRepositoryGetAllActiveExternalIdsReturnsMatchingCampaignExternalIds(externalRefs);
            AssumeCampaignRepositoryGetWithProduct();

            var result = AssumeDeleteByExternalRefsIsRequested(externalRefs) as StatusCodeResult;

            using (new AssertionScope())
            {
                _ = result.StatusCode.Should().Be(HttpStatusCode.NoContent,
                                  "CampaignsController Delete By ExternalRefs Failed to return NoContent result");
                CampaignRepository.Verify(a => a.GetAllActiveExternalIds(), Times.Once(),
                                   "Failed to use CampaignRepository to GetAllActiveExternalIds from the campaigns");
                CampaignCleaner.Verify(a => a.ExecuteAsync(It.IsAny<IReadOnlyCollection<string>>(), default), Times.Once(),
                                   "Failed to use CampaignRepository to delete the campaigns");
            }
        }

        [Test]
        public async Task ReturnNoContentResultWhenSuccessfullyDeletedAndUpdatedDefaultScenarioByRemovingCampaignPassPrioritiesOfDeletedCampaigns()
        {
            var externalRefs = AssumeValidExternalRefsAreSupplied(6);
            AssumeCampaignRepositoryGetAllActiveExternalIdsReturnsMatchingCampaignExternalIds(externalRefs);
            var tenantSettings = AssumeTenantSettingsExistsAndContainsDefaultScenarioId();
            AssumeTenantSettingsRepositoryGetDefaultScenarioIdReturns(tenantSettings.DefaultScenarioId);
            var defaultScenario = CreateValidScenario(tenantSettings.DefaultScenarioId, 10, externalRefs);
            AssumeScenarioRepositoryGetReturnsAScenario(defaultScenario);
            AssumeCampaignRepositoryGetWithProduct();

            var result = AssumeDeleteByExternalRefsIsRequested(externalRefs) as StatusCodeResult;

            using (new AssertionScope())
            {
                _ = result.StatusCode.Should().Be(HttpStatusCode.NoContent,
                                  "CampaignsController Delete By ExternalRefs Failed to return NoContent result");
                CampaignRepository.Verify(a => a.GetAllActiveExternalIds(), Times.Once(),
                                   "Failed to use CampaignRepository to GetAllActiveExternalIds from the campaigns");
                CampaignCleaner.Verify(a => a.ExecuteAsync(It.IsAny<IReadOnlyCollection<string>>(), default), Times.Once(),
                                   "Failed to use CampaignRepository to delete the campaigns");
            }
        }

        [Test]
        public async Task ReturnNoContentResultWhenSuccessfullyDeletedAndNotUpdatedCampaignPassPrioritiesOfDeletedCampaignsIfNoDefaultScenarioExist()
        {
            var externalRefs = AssumeValidExternalRefsAreSupplied(6);
            AssumeCampaignRepositoryGetAllActiveExternalIdsReturnsMatchingCampaignExternalIds(externalRefs);
            var tenantSettings = AssumeTenantSettingsExistsAndDoNotContainDefaultScenarioId();
            AssumeTenantSettingsRepositoryGetDefaultScenarioIdReturns(tenantSettings.DefaultScenarioId);
            AssumeCampaignRepositoryGetWithProduct();

            var result = AssumeDeleteByExternalRefsIsRequested(externalRefs) as StatusCodeResult;

            using (new AssertionScope())
            {
                _ = result.StatusCode.Should().Be(HttpStatusCode.NoContent,
                                  "CampaignsController Delete By ExternalRefs Failed to return NoContent result");
                CampaignRepository.Verify(a => a.GetAllActiveExternalIds(), Times.Once(),
                                   "Failed to use CampaignRepository to GetAllActiveExternalIds from the campaigns");
                CampaignCleaner.Verify(a => a.ExecuteAsync(It.IsAny<IReadOnlyCollection<string>>(), default), Times.Once(),
                                   "Failed to use CampaignRepository to delete the campaigns");
            }
        }

        private IHttpActionResult AssumeDeleteByExternalRefsIsRequested(IEnumerable<string> withExternalRefs)
        {
            return Target.DeleteAsync(withExternalRefs).GetAwaiter().GetResult();
        }

        private List<string> AssumeValidExternalRefsAreSupplied(int noOfExternalRefs)
        {
            return CreateCampaignExternalIds(noOfExternalRefs);
        }

        private List<string> AssumeNoExternalRefsAreSupplied()
        {
            return null;
        }

        private void AssumeDependenciesAreSupplied()
        {
            CampaignRepository = new Mock<ICampaignRepository>();
            TenantSettingsRepository = new Mock<ITenantSettingsRepository>();
            ScenarioRepository = new Mock<IScenarioRepository>();
            MetadataRepository = new Mock<IMetadataRepository>();
            DemographicRepository = new Mock<IDemographicRepository>();
            SalesAreaRepository = new Mock<ISalesAreaRepository>();
            ProductRepository = new Mock<IProductRepository>();
            ReportColumnFormatter = new Mock<IReportColumnFormatter>();
            CampaignExcelReportGenerator = new Mock<ICampaignExcelReportGenerator>();
            PassRepository = new Mock<IPassRepository>();
            ProgrammeCategoryRepository = new Mock<IProgrammeCategoryHierarchyRepository>();
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
