using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AutoFixture;
using ImagineCommunications.GamePlan.CommonTestsHelpers;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.Campaigns.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Moq;
using NUnit.Framework;
using xggameplan.Controllers;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: ScenarioResults")]
    public class ScenarioResultsControllerTests
    {
        private static Fixture _fixture = new SafeFixture();
        private PagedQueryResult<ScenarioCampaignFailure> DummyScenarioCampaignFailures { get; set; }
        private Mock<ICampaignRepository> MockCampaignRepository { get; set; }
        private Mock<IFunctionalAreaRepository> MockFunctionalAreaRepository { get; set; }
        private ScenarioResultsController Controller { get; set; }
        private List<ScenarioCampaignFailureModel> ExtendScenarioCampaignFailureModelResult { get; set; }
        private Mock<ITenantSettingsRepository> MockTenantRepository { get; set; }

        [OneTimeSetUp]
        public async Task OnInit()
        {
            AssumeDependenciesAreSupplied();
            AssumeDependenciesAreMocked();

            Controller = new ScenarioResultsController(null, null, null, null, null, null, null,
                MockCampaignRepository.Object, null, null, null, null, null,
                null, MockFunctionalAreaRepository.Object, MockTenantRepository.Object,
                null, null, null, null, null);

                Controller.Request = new HttpRequestMessage();
            Controller.Configuration = new HttpConfiguration();

            ExtendScenarioCampaignFailureModelResult = Controller.ExtendScenarioCampaignFailureModel(DummyScenarioCampaignFailures.Items.ToList());
        }

        [OneTimeTearDown]
        public async Task OnDestroy() => CleanUp();

        [Test]
        public async Task GivenExtendScenarioCampaignFailureModel_WhenScenarioCampaignFailuresResultArgNull_ThenReturnNotNullObject()
        {
            _ = MockTenantRepository.Setup(o => o.GetStartDayOfWeek()).Returns(DayOfWeek.Sunday);

            var result = Controller.ExtendScenarioCampaignFailureModel(null);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GivenExtendScenarioCampaignFailureModel_WhenScenarioCampaignFailuresResultArgNull_ThenReturnEmptyObject()
        {
            _ = MockTenantRepository.Setup(o => o.GetStartDayOfWeek()).Returns(DayOfWeek.Sunday);

            var result = Controller.ExtendScenarioCampaignFailureModel(null);
            Assert.AreEqual(result.Count, 0);
        }

        [Test]
        public async Task GivenExtendScenarioCampaignFailureModel_WhenScenarioCampaignFailuresResultArgEmptyList_ThenReturnEmptyModel()
        {
            _ = MockTenantRepository.Setup(o => o.GetStartDayOfWeek()).Returns(DayOfWeek.Sunday);

            var result = Controller.ExtendScenarioCampaignFailureModel(new List<ScenarioCampaignFailure>());
            Assert.AreEqual(result.Count, 0);
        }

        [Test]
        public async Task GivenExtendScenarioCampaignFailureModel_WhenScenarioCampaignFailuresResultArgProvided_ThenReturnSameSizeModel()
        {
            _ = MockTenantRepository.Setup(o => o.GetStartDayOfWeek()).Returns(DayOfWeek.Sunday);

            Assert.AreEqual(ExtendScenarioCampaignFailureModelResult.Count, DummyScenarioCampaignFailures.Items.Count);
        }

        private void AssumeDependenciesAreSupplied()
        {
            MockCampaignRepository = new Mock<ICampaignRepository>();
            MockFunctionalAreaRepository = new Mock<IFunctionalAreaRepository>();
            MockTenantRepository = new Mock<ITenantSettingsRepository>();
        }

        private void AssumeDependenciesAreMocked()
        {
            var scenarioId = Guid.NewGuid();
            var campaigns = _fixture.Build<Campaign>().CreateMany(10);
            var campaignIds = campaigns.Select(r => r.ExternalId).Distinct().ToList();
            _ = MockCampaignRepository.Setup(m => m.FindByRefs(campaignIds)).Returns(campaigns);
            var scenarioCampaignFailures = new List<ScenarioCampaignFailure>();
            foreach (var campaign in campaigns)
            {
                scenarioCampaignFailures.Add(_fixture.Build<ScenarioCampaignFailure>()
                    .With(s => s.DayPartDays, "YYYNNYY")
                    .With(s => s.ScenarioId, scenarioId)
                    .With(s => s.ExternalCampaignId, campaign.Id.ToString())
                    .Create());
            }
            DummyScenarioCampaignFailures = new PagedQueryResult<ScenarioCampaignFailure>(20, scenarioCampaignFailures.ToList());
            var failureTypeIds = DummyScenarioCampaignFailures.Items.ToList().Select(r => r.FailureType).Distinct().ToList();
            var failureTypes = failureTypeIds.Select(o => _fixture.Build<FaultType>().With(f => f.Id, o).Create());
            _ = MockFunctionalAreaRepository.Setup(m => m.FindFaultTypes(failureTypeIds)).Returns(failureTypes);
        }

        private void CleanUp()
        {
            _fixture = null;
        }
    }
}
