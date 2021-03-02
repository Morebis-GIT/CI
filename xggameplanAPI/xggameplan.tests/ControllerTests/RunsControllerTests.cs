#pragma warning disable CA1707 // Identifiers should not contain underscores

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Results;
using AutoFixture;
using AutoMapper;
using ImagineCommunications.Gameplan.Synchronization.Interfaces;
using ImagineCommunications.GamePlan.Domain.AnalysisGroups;
using ImagineCommunications.GamePlan.Domain.Autopilot.FlexibilityLevels;
using ImagineCommunications.GamePlan.Domain.Autopilot.Rules;
using ImagineCommunications.GamePlan.Domain.Autopilot.Settings;
using ImagineCommunications.GamePlan.Domain.BusinessRules.RuleTypes;
using ImagineCommunications.GamePlan.Domain.Campaigns;
using ImagineCommunications.GamePlan.Domain.EfficiencySettings;
using ImagineCommunications.GamePlan.Domain.Generic.DbSequence;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Domain.Optimizer.Facilities;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.ISRSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSGlobalSettings;
using ImagineCommunications.GamePlan.Domain.Optimizer.RSSettings;
using ImagineCommunications.GamePlan.Domain.Passes;
using ImagineCommunications.GamePlan.Domain.Runs;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.RunTypes;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics;
using ImagineCommunications.GamePlan.Domain.ScenarioResults;
using ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.Shared.FunctionalAreas;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using ImagineCommunications.GamePlan.Domain.Shared.System.Users;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using xggameplan.Autopilot;
using xggameplan.common.BackgroundJobs;
using xggameplan.Configuration;
using xggameplan.Controllers;
using xggameplan.core.Configuration;
using xggameplan.core.FeatureManagement.Interfaces;
using xggameplan.core.Interfaces;
using xggameplan.core.Landmark.Abstractions;
using xggameplan.model.External;
using xggameplan.Model;
using xggameplan.Reports;
using xggameplan.Reports.DataAdapters;
using xggameplan.Reports.Models;
using xggameplan.Validations;
using xggameplan.Validations.AutopilotSettings;
using xggameplan.Validations.Landmark;
using xggameplan.Validations.Runs.Interfaces;

namespace xggameplan.tests.ControllerTests
{
    [TestFixture(Category = "Controllers :: Run")]
    public class RunsControllerTests : IDisposable
    {
        private Guid _runId;
        private Guid _scenarioId1;
        private Guid _scenarioId2;
        private Mock<RunScenario> _scenario1;
        private Mock<RunScenario> _scenario2;
        private IMapper _mapper;
        private Fixture _fixture;
        private Mock<IScenarioResultRepository> _fakeScenarioResultRepository;
        private Mock<IRunRepository> _fakeRunRepository;
        private Mock<IConfiguration> _fakeConfiguration;
        private Mock<ITenantSettingsRepository> _fakeTenantSettingsRepository;
        private Mock<ISalesAreaRepository> _fakeSalesAreaRepository;
        private Mock<ICampaignRepository> _fakeCampaignRepository;
        private Mock<IISRSettingsRepository> _fakeISRSettingsRepository;
        private Mock<IRSSettingsRepository> _fakeRSSettingsRepository;
        private Mock<IRepositoryFactory> _fakeRepositoryFactory;
        private Mock<ISmoothFailureRepository> _fakeSmoothFailureRepository;
        private Mock<ISmoothFailureMessageRepository> _fakeSmoothFailureMessageRepository;
        private Mock<IDemographicRepository> _fakeDemographicRepository;
        private Mock<AuditEvents.IAuditEventRepository> _fakeAuditEventRepository;
        private Mock<IIdentityGeneratorResolver> _fakeServicesIdentityGeneratorResolver;
        private Mock<RunManagement.IRunManager> _fakeRunManager;
        private Mock<IKPIComparisonConfigRepository> _fakeKPIComparisonConfigRepository;
        private Mock<IScenarioRepository> _fakeScenarioRepository;
        private Mock<IPassRepository> _fakePassRepository;
        private Mock<IEfficiencySettingsRepository> _fakeEfficiencySettingsRepository;
        private Mock<IFunctionalAreaRepository> _fakeFunctionalAreaRepository;
        private Mock<IAutopilotRuleRepository> _fakeAutopilotRuleRepository;
        private Mock<IAutopilotSettingsRepository> _fakeAutopilotSettingsRepository;
        private Mock<IFlexibilityLevelRepository> _fakeFlexibilityLevelRepository;
        private IAutopilotManager _autopilotManager;
        private Mock<IExcelReportGenerator> _fakeExcelReportGenerator;
        private Mock<IRunExcelReportDataAdapter> _fakeRunReportDataAdapter;
        private IModelDataValidator<AutopilotEngageModel> _autopilotEngageModelValidator;
        private Mock<IISRGlobalSettingsRepository> _fakeISRGlobalSettingsRepository;
        private Mock<IRSGlobalSettingsRepository> _fakeRSGlobalSettingsRepository;
        private Mock<IBackgroundJobManager> _fakeBackgroundJobManager;
        private Mock<TenantIdentifier> _fakeTenantIdentifier;
        private Mock<ISynchronizationService> _fakeSynchronizationService;
        private Mock<ILandmarkRunService> _fakeLandmarkRunService;
        private IModelDataValidator<LandmarkRunTriggerModel> _landmarkRunTriggerModelValidator;
        private IModelDataValidator<ScheduledRunSettingsModel> _scheduledRunSettingsModelValidator;
        private Mock<IScenarioCampaignMetricRepository> _scenarioCampaignMetricRepository;
        private Mock<IFeatureManager> _fakeFeatureManager;
        private Mock<IRunsValidator> _fakeRunsValidator;
        private Mock<IAnalysisGroupRepository> _fakeAnalysisGroupRepository;
        private Mock<IUsersRepository> _fakeUsersRepository;
        private Mock<IRunTypeRepository> _fakeRunTypeRepository;
        private Mock<IPassInspectorService> _fakePassInspectorService;
        private Mock<IFacilityRepository> _fakeFacilityRepository;

        private string _kpiName;
        private string _kpi2Name;

        private Mock<Run> _fakeRun;
        private RunsController _controller;

        [SetUp]
        public void Init()
        {
            _fixture = new Fixture();

            _mapper = AutoMapperInitializer.Initialize(AutoMapperConfig.SetupAutoMapper);

            _runId = Guid.NewGuid();
            _scenarioId1 = Guid.NewGuid();
            _scenarioId2 = Guid.NewGuid();
            _scenario1 = new Mock<RunScenario>();
            _scenario2 = new Mock<RunScenario>();

            _fakeConfiguration = new Mock<IConfiguration>();
            _fakeScenarioResultRepository = new Mock<IScenarioResultRepository>();
            _fakeRunRepository = new Mock<IRunRepository>();
            _fakeTenantSettingsRepository = new Mock<ITenantSettingsRepository>();
            _fakeSalesAreaRepository = new Mock<ISalesAreaRepository>();
            _fakeCampaignRepository = new Mock<ICampaignRepository>();
            _fakeISRSettingsRepository = new Mock<IISRSettingsRepository>();
            _fakeRSSettingsRepository = new Mock<IRSSettingsRepository>();
            _fakeISRGlobalSettingsRepository = new Mock<IISRGlobalSettingsRepository>();
            _fakeRSGlobalSettingsRepository = new Mock<IRSGlobalSettingsRepository>();
            _fakeRepositoryFactory = new Mock<IRepositoryFactory>();
            _fakeSmoothFailureRepository = new Mock<ISmoothFailureRepository>();
            _fakeSmoothFailureMessageRepository = new Mock<ISmoothFailureMessageRepository>();
            _fakeDemographicRepository = new Mock<IDemographicRepository>();
            _fakeAuditEventRepository = new Mock<AuditEvents.IAuditEventRepository>();
            _fakeServicesIdentityGeneratorResolver = new Mock<IIdentityGeneratorResolver>();
            _fakeRunManager = new Mock<RunManagement.IRunManager>();
            _fakeKPIComparisonConfigRepository = new Mock<IKPIComparisonConfigRepository>();
            _fakeScenarioRepository = new Mock<IScenarioRepository>();
            _fakePassRepository = new Mock<IPassRepository>();
            _fakeEfficiencySettingsRepository = new Mock<IEfficiencySettingsRepository>();
            _fakeFunctionalAreaRepository = new Mock<IFunctionalAreaRepository>();
            _fakeFlexibilityLevelRepository = new Mock<IFlexibilityLevelRepository>();
            _fakeAutopilotRuleRepository = new Mock<IAutopilotRuleRepository>();
            _fakeAutopilotSettingsRepository = new Mock<IAutopilotSettingsRepository>();
            _fakeExcelReportGenerator = new Mock<IExcelReportGenerator>();
            _fakeRunReportDataAdapter = new Mock<IRunExcelReportDataAdapter>();
            _fakeBackgroundJobManager = new Mock<IBackgroundJobManager>();
            _fakeTenantIdentifier = new Mock<TenantIdentifier>(new object[] { 0, "" });
            _fakeSynchronizationService = new Mock<ISynchronizationService>();
            _fakeLandmarkRunService = new Mock<ILandmarkRunService>();
            _scenarioCampaignMetricRepository = new Mock<IScenarioCampaignMetricRepository>();
            _fakeFeatureManager = new Mock<IFeatureManager>();
            _fakeRunsValidator = new Mock<IRunsValidator>();
            _fakeAnalysisGroupRepository = new Mock<IAnalysisGroupRepository>();
            _fakeRunTypeRepository = new Mock<IRunTypeRepository>();
            _fakeUsersRepository = new Mock<IUsersRepository>();
            _fakePassInspectorService = new Mock<IPassInspectorService>();
            _fakeFacilityRepository = new Mock<IFacilityRepository>();

            _ = _scenario1.Setup(x => x.Id).Returns(_scenarioId1);
            _ = _scenario1.Setup(x => x.Status).Returns(ScenarioStatuses.CompletedSuccess);
            _ = _scenario2.Setup(x => x.Id).Returns(_scenarioId2);
            _ = _scenario2.Setup(x => x.Status).Returns(ScenarioStatuses.CompletedSuccess);

            _kpiName = "KPI1";
            _kpi2Name = "KPI2";
            _fakeRun = new Mock<Run>();

            _autopilotManager =
                new AutopilotManager(_fakeAuditEventRepository.Object, _fakeAutopilotSettingsRepository.Object, _fakeAutopilotRuleRepository.Object, _mapper);
            _autopilotEngageModelValidator =
                new AutopilotEngageModelValidator(new AutopilotEngageModelValidation(_fakeFlexibilityLevelRepository.Object));
            _landmarkRunTriggerModelValidator = new LandmarkRunTriggerModelValidator(new LandmarkRunTriggerModelValidation(_fakeRunRepository.Object));
            _scheduledRunSettingsModelValidator = new ScheduledRunSettingsModelValidator(new ScheduledRunSettingsModelValidation());

            _controller = new RunsController(
                _fakeConfiguration.Object,
                _fakeRunRepository.Object,
                _fakeTenantSettingsRepository.Object,
                _fakeSalesAreaRepository.Object,
                _fakeCampaignRepository.Object,
                _fakeISRSettingsRepository.Object,
                _fakeRSSettingsRepository.Object,
                _fakeRepositoryFactory.Object,
                _fakeEfficiencySettingsRepository.Object,
                _fakeSmoothFailureRepository.Object,
                _fakeDemographicRepository.Object,
                _fakeAuditEventRepository.Object,
                _autopilotManager,
                _fakeServicesIdentityGeneratorResolver.Object,
                _fakeRunManager.Object,
                _mapper,
                _autopilotEngageModelValidator,
                _fakeKPIComparisonConfigRepository.Object,
                _fakeScenarioResultRepository.Object,
                _fakeScenarioRepository.Object,
                _fakePassRepository.Object,
                _fakeFunctionalAreaRepository.Object,
                _fakeExcelReportGenerator.Object,
                _fakeRunReportDataAdapter.Object,
                _fakeISRGlobalSettingsRepository.Object,
                _fakeRSGlobalSettingsRepository.Object,
                _fakeSmoothFailureMessageRepository.Object,
                _fakeBackgroundJobManager.Object,
                _fakeTenantIdentifier.Object,
                _fakeSynchronizationService.Object,
                _fakeLandmarkRunService.Object,
                _landmarkRunTriggerModelValidator,
                _scheduledRunSettingsModelValidator,
                _scenarioCampaignMetricRepository.Object,
                _fakeFeatureManager.Object,
                _fakeRunsValidator.Object,
                _fakeAnalysisGroupRepository.Object,
                _fakeRunTypeRepository.Object,
                _fakeUsersRepository.Object,
                _fakePassInspectorService.Object,
                _fakeFacilityRepository.Object
                );
        }

        [Test]
        [Description("Given valid models when getting metrics then 2 scenario metrics must be returned")]
        public void GetMetricsWhenCalledWithValidModelsThenShouldReturnOk()
        {
            _ = _fakeScenarioResultRepository.Setup(x => x.Find(_scenarioId1)).Returns(
                CreateScenarioResult(_scenarioId1, new Dictionary<string, double> { { _kpiName, 1 }, { _kpi2Name, 5 } })
            );
            _ = _fakeScenarioResultRepository.Setup(x => x.Find(_scenarioId2)).Returns(
                CreateScenarioResult(_scenarioId2, new Dictionary<string, double> { { _kpiName, 3 }, { _kpi2Name, 6 } })
            );
            _ = _fakeKPIComparisonConfigRepository.Setup(x => x.GetAll()).Returns(new List<KPIComparisonConfig>
            {
                new KPIComparisonConfig {KPIName = _kpiName, DiscernibleDifference = 1},
                new KPIComparisonConfig {KPIName = _kpi2Name, DiscernibleDifference = 2}
            });
            _ = _fakeRun.SetupGet(x => x.Scenarios).Returns(new List<RunScenario>
            {
                _scenario1.Object,
                _scenario2.Object
            });
            _ = _fakeRunRepository.Setup(x => x.Find(_runId)).Returns(_fakeRun.Object);

            var actionResult = _controller.GetMetrics(_runId);
            var contentResult = actionResult as OkNegotiatedContentResult<List<ScenarioMetricsResultModel>>;

            Assert.IsTrue(contentResult.Content.Count == 2);
        }

        [Test]
        [Description("Given missing scenario results when getting metrics then 0 scenario metrics must be returned")]
        public void GetMetricsWhenCalledWithNoScenarioResultsThenShouldReturnOk()
        {
            _ = _fakeKPIComparisonConfigRepository.Setup(x => x.GetAll()).Returns(new List<KPIComparisonConfig>
            {
                new KPIComparisonConfig {KPIName = _kpiName, DiscernibleDifference = 1},
                new KPIComparisonConfig {KPIName = _kpi2Name, DiscernibleDifference = 2}
            });
            _ = _fakeRun.SetupGet(x => x.Scenarios).Returns(new List<RunScenario>
            {
                _scenario1.Object,
                _scenario2.Object
            });
            _ = _fakeRunRepository.Setup(x => x.Find(_runId)).Returns(_fakeRun.Object);

            var actionResult = _controller.GetMetrics(_runId);
            var contentResult = actionResult as OkNegotiatedContentResult<List<ScenarioMetricsResultModel>>;

            Assert.IsTrue(contentResult.Content.Count == 0);
        }

        [Test]
        [Description("Given InProgress run status when getting metrics then 2 scenario metrics with 0 rankings must be returned")]
        public void GetMetricsWhenCalledWithInProgressRunStatusThenShouldReturnOkNoRankings()
        {
            _ = _fakeScenarioResultRepository.Setup(x => x.Find(_scenarioId1)).Returns(
                CreateScenarioResult(_scenarioId1, new Dictionary<string, double> { { _kpiName, 1 }, { _kpi2Name, 5 } })
            );
            _ = _fakeScenarioResultRepository.Setup(x => x.Find(_scenarioId2)).Returns(
                CreateScenarioResult(_scenarioId2, new Dictionary<string, double> { { _kpiName, 3 }, { _kpi2Name, 6 } })
            );
            _ = _fakeKPIComparisonConfigRepository.Setup(x => x.GetAll()).Returns(new List<KPIComparisonConfig>
            {
                new KPIComparisonConfig {KPIName = _kpiName, DiscernibleDifference = 1},
                new KPIComparisonConfig {KPIName = _kpi2Name, DiscernibleDifference = 2}
            });

            _ = _scenario1.Setup(x => x.Status).Returns(ScenarioStatuses.InProgress);

            _ = _fakeRun.SetupGet(x => x.Scenarios).Returns(new List<RunScenario>
            {
                _scenario1.Object,
                _scenario2.Object
            });
            _ = _fakeRunRepository.Setup(x => x.Find(_runId)).Returns(_fakeRun.Object);

            var actionResult = _controller.GetMetrics(_runId);
            var contentResult = actionResult as OkNegotiatedContentResult<List<ScenarioMetricsResultModel>>;

            var rankingCount = 0;
            contentResult?.Content.ForEach(x => rankingCount += x.Metrics.Count(metric => metric.Ranking != KPIRanking.None));

            Assert.IsTrue(contentResult.Content.Count == 2);
            Assert.AreEqual(rankingCount, 0);
        }

        [Test]
        public void ExportScenarioPasses_CallsExcelReportGenerator()
        {
            _ = _fakeRun.SetupGet(x => x.Scenarios).Returns(new List<RunScenario>(){
                _scenario1.Object,
                _scenario2.Object
            });

            _ = _fakeRunRepository.Setup(x => x.Find(_runId))
                .Returns(_fakeRun.Object);

            var actionResult = _controller.ExportScenarioPasses(_runId);
            _fakeExcelReportGenerator.Verify(m => m.GetRunExcelReport(It.IsAny<ExcelReportRunModel>()), Times.Once);
        }

        [Test]
        public void ExportScenarioPasses_CallsRunReportDataAdapter()
        {
            _ = _fakeRun.SetupGet(x => x.Scenarios).Returns(new List<RunScenario>(){
                _scenario1.Object,
                _scenario2.Object
            });

            _ = _fakeRunRepository.Setup(x => x.Find(_runId))
                .Returns(_fakeRun.Object);

            var actionResult = _controller.ExportScenarioPasses(_runId);
            _fakeRunReportDataAdapter.Verify(m => m.Map(It.IsAny<RunModel>(), It.IsAny<IEnumerable<Demographic>>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public void ExportSmoothFailure_CallsRunReportDataAdapter()
        {
            _ = _fakeRun.SetupGet(x => x.Scenarios).Returns(new List<RunScenario>(){
                _scenario1.Object,
                _scenario2.Object
            });

            _ = _fakeRunRepository.Setup(x => x.Find(_runId))
                .Returns(_fakeRun.Object);
            _ = _fakeSmoothFailureRepository.Setup(x => x.GetByRunId(_runId))
                .Returns(new List<SmoothFailure>());
            _ = _fakeSmoothFailureMessageRepository.Setup(x => x.GetAll())
                .Returns(new List<SmoothFailureMessage>());
            _ = _fakeSalesAreaRepository.Setup(x => x.GetAll())
                .Returns(new List<SalesArea>());
            var actionResult = _controller.ExportSmoothFailures(_runId);
            _fakeRunReportDataAdapter.Verify(m => m.Map(It.IsAny<Run>(), It.IsAny<List<SmoothFailureModel>>(),
                It.IsAny<IEnumerable<SmoothFailureMessage>>(), It.IsAny<DateTime>(), _mapper), Times.Once);
        }

        [Test]
        [Description("Given valid model when generating autopilot scenarios then set of scenarios must be generated")]
        public void PostAutopilotScenariosWhenCalledWithValidModelThenShouldReturnGeneratedScenarios()
        {
            _ = _fakeFlexibilityLevelRepository.Setup(r => r.Get(1)).Returns(new FlexibilityLevel { Id = 1, Name = "test" });
            _ = _fakeAutopilotRuleRepository.Setup(r => r.GetByFlexibilityLevelId(1)).Returns(CreateValidAutopilotRules());
            _ = _fakeAutopilotSettingsRepository.Setup(r => r.GetDefault()).Returns(CreateAutopilotSettings());

            var actionResult = _controller.PostAutopilotScenarios(BuildModel());
            var contentResult = actionResult as OkNegotiatedContentResult<IList<AutopilotScenarioModel>>;

            Assert.IsTrue(contentResult.Content.Count == 8);
        }

        [Test]
        [Description("Given model without scenarios when generating autopilot scenarios then correct validation message must be returned")]
        [TestCase(nameof(AutopilotEngageModel.FlexibilityLevelId), "FlexibilityLevel with identifier -1 does not exist")]
        [TestCase(nameof(AutopilotEngageModel.Scenarios), "Please provide at least one scenario")]
        public void PostAutopilotScenariosWhenCalledWithMissingScenariosDataThenShouldReturnCorrectValidationMessage(string propertyName, string message)
        {
            var model = BuildModel(flexibilityLevelId: -1, scenariosNumber: 0);
            var actionResult = _controller.PostAutopilotScenarios(model) as ResponseMessageResult;

            AssertValidationMessage(actionResult, propertyName, message);
        }

        [Test]
        [Description("Given model without passes data when generating autopilot scenarios then correct validation message must be returned")]
        [TestCase("Scenarios[0].Passes", "Scenario has no passes")]
        [TestCase("Scenarios[0].TightenPassIndex", "Please specify tighten pass index")]
        [TestCase("Scenarios[0].LoosenPassIndex", "Please specify loosen pass index")]
        public void PostAutopilotScenariosWhenCalledWithMissingPassesDataThenShouldReturnCorrectValidationMessage(string propertyName, string message)
        {
            var model = BuildModel(passesNumber: 0, tightenPassIndex: null, loosenPassIndex: null);
            var actionResult = _controller.PostAutopilotScenarios(model) as ResponseMessageResult;

            AssertValidationMessage(actionResult, propertyName, message);
        }

        [Test]
        [Description("Given model with invalid pass indexes when generating autopilot scenarios then correct validation message must be returned")]
        [TestCase("Scenarios[0].TightenPassIndex", "Specified tighten pass index is not present in the passes collection")]
        [TestCase("Scenarios[0].LoosenPassIndex", "Specified loosen pass index is not present in the passes collection")]
        [TestCase("Scenarios[0].TightenPassIndex", "Tighten pass must go first")]
        public void PostAutopilotScenariosWhenCalledWithInvalidPassIndexesThenShouldReturnCorrectValidationMessage(string propertyName, string message)
        {
            var model = BuildModel(tightenPassIndex: 12, loosenPassIndex: 10);
            var actionResult = _controller.PostAutopilotScenarios(model) as ResponseMessageResult;

            AssertValidationMessage(actionResult, propertyName, message);
        }

        private static void AssertValidationMessage(ResponseMessageResult response, string fieldName, string message)
        {
            Assert.IsTrue(response.Response.StatusCode == HttpStatusCode.BadRequest);

            var errors = (response.Response.Content as ObjectContent<IEnumerable<ErrorModel>>)?.Value as IEnumerable<ErrorModel>;
            Assert.IsTrue(errors.Any(err => err.ErrorField == fieldName && err.ErrorMessage == message));
        }

        private AutopilotEngageModel BuildModel(int scenariosNumber = 1, int passesNumber = 1, int? tightenPassIndex = 0,
            int? loosenPassIndex = 0, int flexibilityLevelId = 1)
        {
            var passes = _fixture.Build<AutopilotPassModel>()
                .With(p => p.Id, 0)
                .With(p => p.Name, "Test pass")
                .With(p => p.General,
                    new List<GeneralModel> { new GeneralModel { RuleId = (int)RuleID.MinimumEfficiency, Value = "1" } })
                .With(p => p.Weightings, new List<WeightingModel> { new WeightingModel() })
                .With(p => p.Tolerances, new List<ToleranceModel> { new ToleranceModel() })
                .With(p => p.Rules, new List<PassRuleModel> { new PassRuleModel() })
                .CreateMany(passesNumber)
                .ToList();

            var scenarios = _fixture.Build<AutopilotScenarioEngageModel>()
                .With(s => s.Name, "Test scenario")
                .With(s => s.TightenPassIndex, tightenPassIndex)
                .With(s => s.LoosenPassIndex, loosenPassIndex)
                .With(s => s.Passes, passes)
                .With(s => s.Status, ScenarioStatuses.Pending)
                .CreateMany(scenariosNumber)
                .ToList();

            return _fixture.Build<AutopilotEngageModel>()
                .With(m => m.FlexibilityLevelId, flexibilityLevelId)
                .With(m => m.Scenarios, scenarios)
                .Create();
        }

        private static ScenarioResult CreateScenarioResult(Guid scenarioId, Dictionary<string, double> values)
        {
            var model = new ScenarioResult
            {
                Id = scenarioId,
                TimeCompleted = DateTime.Now,
                Metrics = new List<KPI>()
            };

            foreach (var kv in values)
            {
                model.Metrics.Add(new KPI
                {
                    Name = kv.Key,
                    Value = kv.Value
                });
            }

            return model;
        }

        private AutopilotSettings CreateAutopilotSettings()
        {
            return _fixture.Build<AutopilotSettings>()
                .With(r => r.Id, 1)
                .With(a => a.DefaultFlexibilityLevelId, 1)
                .With(a => a.ScenariosToGenerate, 8)
                .Create();
        }

        private static IEnumerable<AutopilotRule> CreateValidAutopilotRules(AutopilotFlexibilityLevel flexibilityLevel = AutopilotFlexibilityLevel.Low)
        {
            return new List<AutopilotRule>
            {
                AutopilotRule.Create((int)flexibilityLevel, 1, (int)RuleCategory.General, 5, -5, 10, -10),
                AutopilotRule.Create((int)flexibilityLevel, 2, (int)RuleCategory.General, 5, -5, 10, -10),
                AutopilotRule.Create((int)flexibilityLevel, 3, (int)RuleCategory.General, 0, 0, 0, 0),
                AutopilotRule.Create((int)flexibilityLevel, 1, (int)RuleCategory.Tolerances, 5, 5, 10, 10),
                AutopilotRule.Create((int)flexibilityLevel, 2, (int)RuleCategory.Tolerances, 5, 5, 10, 10),
                AutopilotRule.Create((int)flexibilityLevel, 3, (int)RuleCategory.Tolerances, 5, 5, 10, 10),
                AutopilotRule.Create((int)flexibilityLevel, 1, (int)RuleCategory.Rules, -5, 5, -10, 10),
                AutopilotRule.Create((int)flexibilityLevel, 2, (int)RuleCategory.Rules, 0, 0, 0, 0),
                AutopilotRule.Create((int)flexibilityLevel, 3, (int)RuleCategory.Rules, 0, 0, 0, 0),
                AutopilotRule.Create((int)flexibilityLevel, 1, (int)RuleCategory.SlottingLimits, 0, 0, 0, 0)
            };
        }

        [TearDown]
        public void Cleanup()
        {
            _controller = null;
            _mapper = null;
        }

        public void Dispose()
        {
            _controller = null;
            _mapper = null;
        }
    }
}
#pragma warning restore CA1707 // Identifiers should not contain underscores
