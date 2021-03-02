using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.Products;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Interfaces;
using xggameplan.core.Services;
using xggameplan.Model;
using xggameplan.Reports.Common;
using xggameplan.Reports.ExcelReports.Campaigns;
using xggameplan.Services;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Campaign")]
    public class CampaignsControllerUpdateTests : IDisposable
    {
        private static Fixture _fixture = new SafeFixture();
        private static IMapper _mapper;

        private List<CreateCampaign> _fakeRequest { get; set; }
        private List<Campaign> _fakeCampaign { get; set; }
        private List<Campaign> _fakeExistingCampaign { get; set; }
        private List<Campaign> _fakeNewCampaign { get; set; }

        private PagedQueryResult<CampaignWithProductFlatModel> _fakeSearchCampaignResult { get; set; }

        [SetUp]
        public void Init()
        {
            _fixture = new SafeFixture();
            _mapper = AutoMapperInitializer.Initialize(Configuration.AutoMapperConfig.SetupAutoMapper);
        }

        private CampaignsController CreateCampaignController(
            Mock<IDataChangeValidator> mockDataChangeValidator,
            Mock<IRecommendationRepository> mockRecommendationRepository,
            Mock<IDemographicRepository> mockDemographicRepository,
            Mock<ICampaignRepository> mockCampaignRepository,
            Mock<IProgrammeCategoryHierarchyRepository> mockProgrammeCategoryRepository,
            Mock<ISalesAreaRepository> mockSalesAreaRepository,
            Mock<IProductRepository> mockProductRepository,
            Mock<ICampaignExcelReportGenerator> mockCampaignExportReportGenerator,
            Mock<IReportColumnFormatter> mockReportColumnFormatter,
            Mock<IFeatureManager> mockFeatureManager,
            Mock<ICampaignCleaner> mockCampaignCleaner,
            Mock<ICampaignPassPrioritiesService> mockCampaignPassPrioritiesService)
        {
            var flattener = new CampaignFlattener(mockProductRepository.Object, mockDemographicRepository.Object,
                null, _mapper);
            return new CampaignsController(mockCampaignRepository.Object,
                 mockRecommendationRepository.Object, mockDataChangeValidator.Object,
                 _mapper, mockDemographicRepository.Object,
                 mockSalesAreaRepository.Object, mockProductRepository.Object,
                 mockCampaignExportReportGenerator.Object, mockReportColumnFormatter.Object,
                 null, null, mockProgrammeCategoryRepository.Object, mockFeatureManager.Object,
                 flattener, mockCampaignCleaner.Object, mockCampaignPassPrioritiesService.Object);
        }

        private void CreateFakeData(int requestCount, int repositoryCount, int existingRecordCount)
        {
            _fakeRequest = _fixture
                .Build<CreateCampaign>()
                .With(c => c.DeliveryType, CampaignDeliveryType.Rating.ToString())
                .CreateMany(requestCount)
                .ToList();
            _fakeCampaign = _fixture.Build<Campaign>().CreateMany(repositoryCount).ToList();
            var fakeExistingCreateCampaign = _fakeRequest.Take(existingRecordCount);
            var fakeNewCreateCampaign = _fakeRequest.Skip(existingRecordCount);
            var customId = _fakeCampaign.Select(x => x.CustomId).Max();

            Campaign dummyCampaign;
            _fakeExistingCampaign = new List<Campaign>();
            foreach (var existingCampaign in fakeExistingCreateCampaign)
            {
                dummyCampaign = _mapper.Map<Campaign>(existingCampaign);
                dummyCampaign.CustomId = ++customId;
                dummyCampaign.Id = new Guid();
                _fakeExistingCampaign.Add(dummyCampaign);
            }
            _fakeCampaign.AddRange(_fakeExistingCampaign);

            _fakeNewCampaign = new List<Campaign>();
            foreach (var newCampaign in fakeNewCreateCampaign)
            {
                dummyCampaign = _mapper.Map<Campaign>(newCampaign);
                dummyCampaign.CustomId = ++customId;
                dummyCampaign.Id = new Guid();
                _fakeNewCampaign.Add(dummyCampaign);
            }

            _fakeSearchCampaignResult = new PagedQueryResult<CampaignWithProductFlatModel>(0, new List<CampaignWithProductFlatModel>());
        }

        [Test]
        [Description("Update campaign with CampaignCreate object list which includes new and existing object parameters then should return ok")]
        public void GivenUpdateCampaign_WhenCalledCreateCampaignListWhichIncludesNewAndExistingRecords_ThenShouldReturnOk()
        {
            //Arrange
            CreateFakeData(10, 15, 6);

            var mockCampaignRepository = new Mock<ICampaignRepository>();
            _ = mockCampaignRepository.Setup(m => m.GetAll()).Returns(_fakeCampaign);
            _ = mockCampaignRepository.Setup(m => m.GetWithProduct(null)).Returns(_fakeSearchCampaignResult);
            _ = mockCampaignRepository.Setup(m => m.FindByRefs(It.IsAny<List<string>>())).Returns(_fakeExistingCampaign);
            _ = mockCampaignRepository.Setup(m => m.Add(It.IsAny<List<Campaign>>())).Callback(AddNewCampaigns);
            _ = mockCampaignRepository.Setup(m => m.Update(It.IsAny<Campaign>()));

            var mockCampaignNoIdentities = _fixture.Build<CampaignNoIdentity>().CreateMany(_fakeRequest.Count - _fakeExistingCampaign.Count).ToList();

            var expectedReposistoryRecordCount = _fakeRequest.Count + _fakeCampaign.Count - _fakeExistingCampaign.Count;

            using (var controller = CreateCampaignController(new Mock<IDataChangeValidator>(),
                new Mock<IRecommendationRepository>(),
                new Mock<IDemographicRepository>(),
                mockCampaignRepository,
                new Mock<IProgrammeCategoryHierarchyRepository>(),
                new Mock<ISalesAreaRepository>(),
                new Mock<IProductRepository>(),
                new Mock<ICampaignExcelReportGenerator>(),
                new Mock<IReportColumnFormatter>(),
                new Mock<IFeatureManager>(),
                new Mock<ICampaignCleaner>(),
                new Mock<ICampaignPassPrioritiesService>()))
            {
                // Act
                var result = controller.Put(_fakeRequest) as OkResult;

                // Assert
                Assert.That(expectedReposistoryRecordCount, Is.EqualTo(_fakeCampaign.Count()));

                Assert.That(_fakeNewCampaign.Select(e => e.AchievedPercentageTargetRatings), Is.All.Not.Null);
                Assert.That(_fakeNewCampaign.Select(e => e.ActiveLength), Is.All.Not.Null);
                Assert.That(_fakeNewCampaign.Select(e => e.RatingsDifferenceExcludingPayback), Is.All.Not.Null);

                Assert.IsNotNull(result);
            }
        }

        private void AddNewCampaigns()
        {
            foreach (var campaign in _fakeNewCampaign)
            {
                campaign.UpdateDerivedKPIs();
            }
            _fakeCampaign.AddRange(_fakeNewCampaign);
        }

        [TearDown]
        public void Cleanup()
        {
            _fixture = null;
            _mapper = null;
        }

        public void Dispose()
        {
            _fixture = null;
            _mapper = null;
        }
    }
}
