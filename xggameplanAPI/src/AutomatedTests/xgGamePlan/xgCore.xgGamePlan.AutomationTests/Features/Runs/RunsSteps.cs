using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.AutopilotSettings;
using xgCore.xgGamePlan.ApiEndPoints.Models.Passes;
using xgCore.xgGamePlan.ApiEndPoints.Models.Runs;
using xgCore.xgGamePlan.ApiEndPoints.Models.Scenarios;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Coordinators;

namespace xgCore.xgGamePlan.AutomationTests.Features.Runs
{
    [Binding]
    public class RunsSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly IRunsApi _runsApi;
        private readonly ITestEnvironmentMaintenanceApi _testEnvironmentMaintenanceApi;
        private readonly RunsCoordinator _runsCoordinator;
        private readonly IFixture _fixture;

        public RunsSteps(ScenarioContext scenarioContext, IRunsApi runsApi,
            ITestEnvironmentMaintenanceApi testEnvironmentMaintenanceApi, RunsCoordinator runsCoordinator,
            IFixture fixture)
        {
            _scenarioContext = scenarioContext;
            _runsApi = runsApi;
            _testEnvironmentMaintenanceApi = testEnvironmentMaintenanceApi;
            _runsCoordinator = runsCoordinator;
            _fixture = fixture;
        }

        [Given(@"I have (.*) valid Run")]
        public async Task GivenIHaveValidRun(int count)
        {
            var runs = await _runsCoordinator.CreateRunsAsync(count).ConfigureAwait(false);
            _scenarioContext.Set(runs);
        }

        [Given(@"I have added (.*) Run")]
        public async Task GivenIHaveAddedRun(int count)
        {
            var runs = await _runsCoordinator.CreateRunsAsync(count).ConfigureAwait(false);
            _scenarioContext.Set(runs);
        }

        [Given(@"I know how many Runs there are")]
        public async Task GivenIKnowHowManyRunsThereAre()
        {
            var runs = await _runsApi.GetAll().ConfigureAwait(false);
            _scenarioContext.Set(runs.Count);
        }

        [Given(@"I have a valid AutopilotEngage model")]
        public void GivenIHaveAValidAutopilotEngageModel()
        {
            _scenarioContext.Set(BuildAutopilotEngageModel(), "autopilotEngageModel");
        }

        [When(@"I generate autopilot scenarios")]
        public async Task WhenIGenerateAutopilotScenarios()
        {
            var model = _scenarioContext.Get<AutopilotEngageModel>("autopilotEngageModel");
            var scenarios = await _runsApi.PostAutopilotScenarios(model).ConfigureAwait(false);

            _scenarioContext.Set(scenarios, "generatedScenarios");
        }

        [Then(@"Correct number of autopilot scenarios is returned")]
        public void ThenCorrectNumberOfAutopilotScenariosIsReturned()
        {
            var scenarios = _scenarioContext.Get<List<Scenario>>("generatedScenarios");
            Assert.That(scenarios.Count, Is.EqualTo(_scenarioContext.Get<int>()));
        }

        [When(@"I add (.*) Runs")]
        public async Task WhenIAddRuns(int count)
        {
            await _runsCoordinator.CreateRunsAsync(count).ConfigureAwait(false);
        }

        [When(@"I search my Run by Description")]
        public async Task WhenISearchMyRunByDescription()
        {
            var run = _scenarioContext.Get<IEnumerable<Run>>().First();
            var runSearchResult = await _runsApi.Search(new RunSearchQueryModel { Description = run.Description }).ConfigureAwait(false);

            _scenarioContext.Set(runSearchResult);
        }

        [Then(@"(.*) additional Runs are returned")]
        public async Task ThenAdditionalRunsAreReturned(int count)
        {
            var runs = await _runsApi.GetAll().ConfigureAwait(false);

            int existingRuns = _scenarioContext.Get<int>();
            Assert.That(runs.Count, Is.EqualTo(existingRuns + count));
        }

        [Then(@"requested RunSearchResult with Description is found")]
        public void ThenRequestedRunSearchResultWithDescriptionIsFound()
        {
            var run = _scenarioContext.Get<IEnumerable<Run>>().First();
            var runSearchResult = _scenarioContext.Get<SearchResultModel<RunSearchResult>>();

            Assert.AreEqual(run.Description, runSearchResult.Items.First().Description);
        }
        
        [Given(@"I have added (.*) Test Run Result")]
        public async Task GivenIHaveAddedTestRunResult(int count)
        {
            await AddTestRunResult(count).ConfigureAwait(false);
        }

        [When(@"I request my Run by ID")]
        public async Task WhenIRequestMyRunByID()
        {
            var testRunResults = _scenarioContext.Get<List<TestEnvironmentRunResult>>();
            var runResult = await _runsApi.GetById(testRunResults.First().RunId).ConfigureAwait(false);
            _scenarioContext.Set(runResult);
        }

        [Then(@"requested Run with ID is returned")]
        public void ThenRequestedRunWithIDIsReturned()
        {
            var testRunResult = _scenarioContext.Get<List<TestEnvironmentRunResult>>();
            var runResult = _scenarioContext.Get<Run>();

            Assert.AreEqual(testRunResult.First().RunId, runResult.Id);
        }

        [When(@"I update Run by ID")]
        public async Task WhenIUpdateRunByID()
        {
            var runResults = _scenarioContext.Get<IEnumerable<Run>>();
            var runResult = await _runsApi.GetById(runResults.First().Id).ConfigureAwait(false);
            runResult.Description = "Updated Run";
            await _runsApi.UpdateById(runResult.Id, runResult).ConfigureAwait(false);
            _scenarioContext.Set(runResult);
        }

        [Then(@"updated Run is returned")]
        public async Task ThenUpdatedRunIsReturned()
        {
            var updatedRun = _scenarioContext.Get<Run>();
            var run = await _runsApi.GetById(updatedRun.Id).ConfigureAwait(false);

            Assert.AreEqual(run.Id, updatedRun.Id);
            Assert.AreEqual(run.Description, updatedRun.Description);
        }

        [When(@"I remove my Run by ID")]
        public async Task WhenIRemoveMyRunByID()
        {
            var testRunResults = _scenarioContext.Get<IEnumerable<Run>>();
            foreach(var run in testRunResults)
            {
                await _runsApi.DeleteById(run.Id).ConfigureAwait(false);
            }
        }

        [Then(@"no Runs with ID is returned")]
        public async Task ThenNoRunsWithIDIsReturned()
        {
            var testRunResults = _scenarioContext.Get<List<TestEnvironmentRunResult>>();
            foreach (var testRunResult in testRunResults)
            {
                try
                {
                    var run = await _runsApi.GetById(testRunResult.RunId).ConfigureAwait(false);
                    Assert.Fail();
                }
                catch (ApiException e)
                {
                    Assert.That(e.StatusCode == HttpStatusCode.NotFound);
                }
            }
        }

        [When(@"I add (.*) Test Run Results")]
        public async Task WhenIAddTestRunResults(int count)
        {
            await AddTestRunResult(count).ConfigureAwait(false);
        }

        [When(@"I request metrics by Run ID")]
        public async Task WhenIRequestMetricsByRunID()
        {
            var runId = _scenarioContext.Get<List<TestEnvironmentRunResult>>().FirstOrDefault()?.RunId ?? Guid.Empty;
            var metrics = await _runsApi.GetMetrics(runId).ConfigureAwait(false);
            _scenarioContext.Set(metrics);
        }

        [Then(@"requested metrics are returned")]
        public void ThenRequestedMetricsAreReturned()
        {
            var metrics = _scenarioContext.Get<IEnumerable<ScenarioMetricsResultModel>>().FirstOrDefault();
            var runResult = _scenarioContext.Get<List<TestEnvironmentRunResult>>().FirstOrDefault();

            Assert.Multiple(() =>
            {
                Assert.That(metrics.ScenarioId == runResult.ScenarioId);
                Assert.That(metrics.Metrics.Any);
            });
        }

        private async Task AddTestRunResult(int count)
        {
            var runResults = new List<TestEnvironmentRunResult>();
            for (int i = 0; i < count; i++)
            {
                var runResult = await _testEnvironmentMaintenanceApi.PopulateRunResultData().ConfigureAwait(false);
                runResults.Add(runResult);
            }

            _scenarioContext.Set(runResults);
        }

        private AutopilotEngageModel BuildAutopilotEngageModel()
        {
            var passes = _fixture.Build<Pass>()
                .With(p => p.Id, 0)
                .With(p => p.Name, "Test pass")
                .With(p => p.General, new List<GeneralModel> { new GeneralModel() })
                .With(p => p.Weightings, new List<WeightingModel> { new WeightingModel() })
                .With(p => p.Tolerances, new List<ToleranceModel> { new ToleranceModel() })
                .With(p => p.Rules, new List<RuleModel> { new RuleModel() })
                .CreateMany(1)
                .ToList();

            var scenarios = _fixture.Build<AutopilotScenarioEngageModel>()
                .With(s => s.Name, "Test scenario")
                .With(s => s.TightenPassIndex, 0)
                .With(s => s.LoosenPassIndex, 0)
                .With(s => s.Passes, passes)
                .CreateMany(1)
                .ToList();

            return _fixture.Build<AutopilotEngageModel>()
                .With(m => m.FlexibilityLevelId, 1)
                .With(m => m.Scenarios, scenarios)
                .Create();
        }
    }
}
