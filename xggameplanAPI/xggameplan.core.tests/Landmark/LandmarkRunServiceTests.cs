using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared;
using Moq;
using NodaTime;
using NUnit.Framework;
using xggameplan.AuditEvents;
using xggameplan.core.Hubs;
using xggameplan.core.Landmark;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.KPIProcessing.Abstractions;
using xggameplan.model.Internal.Landmark;
using xggameplan.Model;

namespace xggameplan.core.tests.Landmark
{
    [TestFixture]
    public class LandmarkRunServiceTests
    {
        private Mock<ILandmarkApiClient> _mockLandmarkApi;
        private Mock<IRunRepository> _mockRunRepository;
        private Mock<IScenarioResultRepository> _mockScenarioResultRepository;
        private Mock<ILandmarkAutoBookPayloadProvider> _mockLandmarkAutobookPayloadProvider;
        private Mock<IHubNotification<LandmarkRunStatusNotification>> _mockCompletedRunNotification;
        private Mock<ILandmarkHttpPoliciesHolder> _mockHttpPoliciesHolder;
        private Mock<IMapper> _mockMapper;
        private Mock<IAuditEventRepository> _mockAuditEventRepository;
        private Mock<IKPICalculationScopeFactory> _mockKpiCalculationScopeFactory;
        private Mock<IKPICalculationManager> _mockKpiCalculationManager;

        private ILandmarkRunService _landmarkRunService;
        private Fixture _fixture;

        [SetUp]
        public void Init()
        {
            _fixture = new Fixture();
            _mockLandmarkApi = new Mock<ILandmarkApiClient>();
            _mockRunRepository = new Mock<IRunRepository>();
            _mockScenarioResultRepository = new Mock<IScenarioResultRepository>();
            _mockLandmarkAutobookPayloadProvider = new Mock<ILandmarkAutoBookPayloadProvider>();
            _mockCompletedRunNotification = new Mock<IHubNotification<LandmarkRunStatusNotification>>();
            _mockHttpPoliciesHolder = new Mock<ILandmarkHttpPoliciesHolder>();
            _mockMapper = new Mock<IMapper>();
            _mockAuditEventRepository = new Mock<IAuditEventRepository>();
            _mockKpiCalculationScopeFactory = new Mock<IKPICalculationScopeFactory>();
            _mockKpiCalculationManager = new Mock<IKPICalculationManager>();

            var kpiCalculationScope = new Mock<IKPICalculationScope>();
            var kpiCalculationContext = new Mock<IKPICalculationContext>();

            _ = _mockLandmarkApi
                .Setup(x => x.TriggerRunAsync(It.IsAny<LandmarkBookingRequest>(), It.IsAny<ScheduledRunSettingsModel>()))
                .Returns(Task.FromResult(new LandmarkBookingResponseModel()));
            _ = _mockLandmarkApi.Setup(x => x.GetRunStatusAsync(It.IsAny<Guid>())).Returns(Task.FromResult(new LandmarkJobStatusModel()));
            _ = _mockHttpPoliciesHolder.SetupAllProperties();

            _ = kpiCalculationContext.SetupAllProperties();
            _ = kpiCalculationScope.Setup(x => x.Resolve<IKPICalculationContext>()).Returns(kpiCalculationContext.Object);
            _ = kpiCalculationScope.Setup(x => x.Resolve<IKPICalculationManager>()).Returns(_mockKpiCalculationManager.Object);

            _ = _mockKpiCalculationScopeFactory.Setup(x => x.CreateCalculationScope(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Returns(kpiCalculationScope.Object);

            _landmarkRunService = new LandmarkRunService(
                _mockLandmarkApi.Object,
                _mockRunRepository.Object,
                _mockScenarioResultRepository.Object,
                _mockLandmarkAutobookPayloadProvider.Object,
                _mockCompletedRunNotification.Object,
                new Mock<IHubNotification<InfoMessageNotification>>().Object,
                _mockHttpPoliciesHolder.Object,
                _mockAuditEventRepository.Object,
                SystemClock.Instance,
                _mockKpiCalculationScopeFactory.Object,
                _mockMapper.Object);
        }

        [Test]
        [Description("Given valid LandmarkRunTriggerModel when triggering run then method must execute successfully")]
        public void TriggerRunWhenCalledWithValidModelThenShouldNotThrowException()
        {
            var scenarioId = Guid.NewGuid();
            var command = new LandmarkRunTriggerModel
            {
                ScenarioId = scenarioId
            };

            _ = _mockRunRepository.Setup(x => x.FindByScenarioId(scenarioId)).Returns(GetRunWithScenario(scenarioId));

            _landmarkRunService.Awaiting(async x => await x.TriggerRunAsync(command).ConfigureAwait(false)).Should().NotThrow();
        }

        [Test]
        [Description("Given valid LandmarkRunTriggerModel when triggering run then method must call Landmark booking Api")]
        public async Task TriggerRunWhenCalledWithValidModelThenShouldCallLandmarkBookingApi()
        {
            var scenarioId = Guid.NewGuid();
            var command = new LandmarkRunTriggerModel
            {
                ScenarioId = scenarioId
            };

            _ = _mockRunRepository.Setup(x => x.FindByScenarioId(scenarioId)).Returns(GetRunWithScenario(scenarioId));
            await _landmarkRunService.TriggerRunAsync(command).ConfigureAwait(false);

            _mockLandmarkApi.Verify(x => x.TriggerRunAsync(It.IsAny<LandmarkBookingRequest>(), null), Times.Once);
        }

        [Test]
        [Description("Given null command value when triggering run then exception must be thrown")]
        public void TriggerRunWhenCalledWithNullCommandValueThenShouldThrowException()
        {
            _ = _landmarkRunService.Awaiting(async x => await x.TriggerRunAsync(default).ConfigureAwait(false)).Should().Throw<ArgumentNullException>();
        }

        [Test]
        [Description("Given empty scenario id value when triggering run then exception must be thrown")]
        public void TriggerRunWhenCalledWithEmptyScenarioIdThenShouldThrowException()
        {
            _ = _landmarkRunService.Awaiting(async x => await x.TriggerRunAsync(_fixture.Create<LandmarkRunTriggerModel>()).ConfigureAwait(false)).Should().Throw<Exception>();
        }

        [Test]
        [Description("When probe landmark availability is called then must execute successfully")]
        public void ProbeLandmarkAvailabilityAsyncWhenCalledThenShouldNotThrowException()
        {
            _ = _mockHttpPoliciesHolder.SetupProperty(x => x.LandmarkIsNotAvailable, true);
            _ = _mockLandmarkApi.Setup(x => x.ProbeLandmarkAsync()).Returns(Task.FromResult(true));

            _landmarkRunService.Awaiting(async x => await x.ProbeLandmarkAvailabilityAsync().ConfigureAwait(false)).Should().NotThrow();
        }

        [Test]
        [Description("Given nonexistent scenario id when triggering run then exception must be thrown")]
        public void TriggerRunWhenCalledWithNonExistentScenarioIdThenShouldThrowException()
        {
            var scenarioId = Guid.NewGuid();
            var command = new LandmarkRunTriggerModel
            {
                ScenarioId = scenarioId
            };

            _ = _mockRunRepository.Setup(x => x.FindByScenarioId(It.IsNotIn(scenarioId))).Returns(_fixture.Create<Run>());

            _ = _landmarkRunService.Awaiting(async x => await x.TriggerRunAsync(command).ConfigureAwait(false)).Should().Throw<Exception>();
        }

        [Test]
        [Description("Given external id for completed run when checking run status then results must be processed successfully")]
        public async Task UpdateLastExecutedRunStatusWhenCalledForCompletedRunThenShouldProcessResultsSuccessfully()
        {
            var scenario = _fixture.Build<RunScenario>()
                .With(x => x.ExternalRunInfo, new ExternalRunInfo
                {
                    ExternalStatus = ExternalScenarioStatus.Accepted,
                    ExternalRunId = Guid.NewGuid()
                })
                .Create();

            var run = _fixture.Build<Run>()
                .With(x => x.Scenarios, new List<RunScenario> { scenario })
                .Create();

            var jobStatus = _fixture.Build<LandmarkJobStatusModel>()
                .With(x => x.JobStatus, ExternalScenarioStatus.Completed)
                .With(x => x.OutputFiles, PrepareTestOutputFilesData())
                .Create();

            _ = _mockRunRepository.Setup(x => x.FindTriggeredInLandmark()).Returns(new[] { run });

            _ = _mockLandmarkApi.Setup(x => x.GetRunStatusAsync(It.IsAny<Guid>())).Returns(Task.FromResult(jobStatus));

            _ = _mockKpiCalculationManager.Setup(x => x.SetAudit(It.IsAny<IAuditEventRepository>()))
                .Returns(_mockKpiCalculationManager.Object);
            _ = _mockKpiCalculationManager
                .Setup(x => x.CalculateKPIs(It.IsAny<HashSet<string>>(), run.Id, scenario.Id))
                .Returns(new List<KPI>(0));

            await _landmarkRunService.UpdateRunStatusesAsync().ConfigureAwait(false);

            _mockKpiCalculationScopeFactory.Verify(x => x.CreateCalculationScope(run.Id, scenario.Id), Times.Once);
            _mockScenarioResultRepository.Verify(x => x.Add(It.IsAny<ScenarioResult>()), Times.Once);
            _mockScenarioResultRepository.Verify(x => x.SaveChanges(), Times.Once);
        }

        [TearDown]
        public void Cleanup()
        {
            _landmarkRunService = null;
            _fixture = null;
        }

        private Run GetRunWithScenario(Guid scenarioId)
        {
            var run = _fixture.Create<Run>();
            run.Scenarios.Add(_fixture.Build<RunScenario>().With(x => x.Id, scenarioId).Create());

            return run;
        }

        private static List<LandmarkOutputFileModel> PrepareTestOutputFilesData()
        {
            return new List<LandmarkOutputFileModel>
            {
                new LandmarkOutputFileModel
                {
                    FileName = "kpi.out",
                    Payload = "6,0,1,1,0,0,0,3,0,0,3,1,1,2,0,9,8,9,16,31," +
                              "159,36,17,59,113.25,16632,49266966.67,216430," +
                              "105.56,92.73,125219116.67,564205"
                },
                new LandmarkOutputFileModel
                {
                    FileName = "lmk_kpi.out",
                    Payload = "15033,211,1388,16632,0,0,0,0,0,0,0,0,0.000000,0.000000,0,0," +
                              "2810440.859926,2810440.859926,-3339559.140074,45.698225," +
                              "-3339559.140074,45.698225,6150000.000000,0.000000"
                },
                new LandmarkOutputFileModel
                {
                    FileName = "conv_eff.out",
                    Payload = @"1,86926450,207315700,0,0,102.577958,41.929507,40.875747
                                1,3600500,3600500,21171.85374,21171.85374,100,100,100
                                1,591900,5164000,4327.468636,21171.85374,20.439725,11.462045,56.077296
                                2,188800,941050,8576.625779,21171.85374,40.509565,20.062696,49.525825
                                2,3115950,3506550,36436.59385,21171.85374,172.099214,88.860846,51.633499
                                2,0,0,9153.133715,21171.85374,43.232557,0,0
                                3,0,0,15125.16041,21171.85374,71.439944,0,0
                                3,0,0,6056.847033,21171.85374,28.608015,0,0
                                3,0,0,13475.1766,21171.85374,63.646655,0,0
                                4,1060600,1124950,28102.68624,21171.85374,132.736068,94.279746,71.027978
                                4,0,0,3716.841424,21171.85374,17.555579,0,0
                                4,151100,368450,31627.39787,21171.85374,149.384169,41.009635,27.452464
                                5,0,0,5966.175048,21171.85374,28.179748,0,0
                                5,0,0,36358.27731,21171.85374,171.729305,0,0
                                5,26575700,62126400,18600.87925,21171.85374,87.85664,42.776823,48.689345
                                7,1047100,4352000,18592.98844,21171.85374,87.819369,24.060202,27.397375
                                7,0,0,10926.66465,21171.85374,51.60939,0,0
                                7,0,0,16732.11721,21171.85374,79.030006,0,0"
                },
                new LandmarkOutputFileModel
                {
                    FileName = "resv_rtgs.out",
                    Payload = @"2,1,521.07919,3549550,581050,0
                              3,1,359.45735,2967450,1125600,0
                              4,1,431.91814,2575050,635950,0
                              8,2,1.00258,0,0,0
                              9,2,5.23633,200,0,0
                              10,2,294.54311,103700,0,0
                              20,3,706.68863,413333.3333,280833.3333,0
                              21,3,250.92152,6600,0,0
                              22,3,1.76113,3200,0,0
                              3,4,450.63473,1474650,554800,0
                              4,4,427.76668,979100,235350,0
                              5,4,313.51351,1796600,425950,0
                              22,5,447.602,2295650,456350,0
                              23,5,2166.51813,3267850,1491900,0
                              24,5,2839.30872,6219300,3972100,0
                              2,7,28.18872,12250,0,0
                              22,7,48.03085,45150,4400,0
                              23,7,929.46821,340950,176750,0"
                }
            };
        }
    }
}
