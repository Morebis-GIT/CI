using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Refit;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Contexts;

namespace xgCore.xgGamePlan.AutomationTests.Features.ScenarioResults
{
    [Binding]
    public class ScenarioResultsSteps
    {
        private readonly ITestEnvironmentMaintenanceApi _testEnvironmentMaintenanceApi;
        private readonly IScenarioResultsApi _scenarioResultsApi;
        private readonly ScenarioResultsContext _scenarioResultsContext;

        public ScenarioResultsSteps(
            ITestEnvironmentMaintenanceApi testEnvironmentMaintenanceApi,
            IScenarioResultsApi scenarioResultsApi,
            ScenarioResultsContext scenarioResultsContext)
        {
            _testEnvironmentMaintenanceApi = testEnvironmentMaintenanceApi;
            _scenarioResultsApi = scenarioResultsApi;
            _scenarioResultsContext = scenarioResultsContext;
        }

        [Given(@"I have added Test Run Result")]
        public async Task IHaveAddedTestRunResult()
        {
            _scenarioResultsContext
                .RunResultData = await _testEnvironmentMaintenanceApi
                    .PopulateRunResultData()
                    .ConfigureAwait(false);
        }

        [When(@"I request result for scenario")]
        public async Task WhenIRequestResultForScenario()
        {
            var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;
            var scenarioResult = await _scenarioResultsApi
                .GetResultByScenarioId(scenarioId)
                .ConfigureAwait(false);
            _scenarioResultsContext.ScenarioResult = scenarioResult;
        }

        [Then(@"result is returned")]
        public void ThenResultIsReturned()
        {
            var scenarioResult = _scenarioResultsContext.ScenarioResult;
            var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;

            Assert.IsNotNull(scenarioResult);
            Assert.AreEqual(scenarioResult.Id, scenarioId);
        }

        [When(@"I request top failures for scenario")]
        public async Task WhenIRequestTopFailuresFroScenario()
        {
            var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;
            _scenarioResultsContext.TopFailures = await _scenarioResultsApi
                .GetTopFailures(scenarioId)
                .ConfigureAwait(false);
        }

        [Then(@"top failures are returned")]
        public void ThenTopFailuresAreReturned()
        {
            Assert.IsNotEmpty(_scenarioResultsContext.TopFailures);
        }

        [When(@"I request failures for scenario")]
        public async Task WhenIRequestFailuresForScenario()
        {
            var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;
            _scenarioResultsContext.Failures = await _scenarioResultsApi
                .GetFailures(scenarioId)
                .ConfigureAwait(false);
        }

        [Then(@"failures are returned")]
        public void ThenFailuresAreReturned()
        {
            Assert.IsNotEmpty(_scenarioResultsContext.Failures);
        }

        [When(@"I request metrics for scenario")]
        public async Task WhenIRequestMetricsFroScenario()
        {
            var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;
            _scenarioResultsContext.Metrics = await _scenarioResultsApi
                .GetMetrics(scenarioId)
                .ConfigureAwait(false);
        }

        [Then(@"metrics are returned")]
        public void ThenMetricsAreReturned()
        {
            Assert.IsNotNull(_scenarioResultsContext.Metrics);
        }

        [When(@"I request grouped results by (.*) for scenario")]
        public async Task WhenIRequestGroupedResultsFroScenario(string groupbynames)
        {
            var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;
            _scenarioResultsContext.GroupResult = await _scenarioResultsApi
                .GetDrilldown(scenarioId, groupbynames)
                .ConfigureAwait(false);
        }

        [Then(@"grouped result is returned")]
        public void ThenGroupedResultIsReturned()
        {
            Assert.IsNotEmpty(_scenarioResultsContext.GroupResult);
        }

        [When(@"I delete scenario result")]
        public async Task WhenIDeleteScenarioResult()
        {
            var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;
            var result = await _scenarioResultsApi
                .DeleteResultByScenarioId(scenarioId)
                .ConfigureAwait(false);
        }

        [Then(@"no result is returned")]
        public async Task ThenNoResultIsReturned()
        {
            try
            {
                var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;
                var result = await _scenarioResultsApi
                    .GetResultByScenarioId(scenarioId)
                    .ConfigureAwait(false);
                Assert.Fail();
            }
            catch (ApiException e)
            {
                Assert.That(e.StatusCode == HttpStatusCode.NotFound);
            }
        }

        [When(@"I request simple recommendations for scenarios")]
        public async Task WhenIRequestSimpleRecommendationsForScenarios()
        {
            var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;
            _scenarioResultsContext.RecommendationsForScenarios = await _scenarioResultsApi
                .GetSimleRecommendationsForScenarios(new Guid[] { scenarioId })
                .ConfigureAwait(false);
        }

        [Then(@"simple recommendations are returned")]
        public void ThenSimpleRecommendationsAreReturned()
        {
            Assert.IsNotEmpty(_scenarioResultsContext.RecommendationsForScenarios);
        }

        [When(@"I request simple recommendations for scenario")]
        public async Task WhenIRequestSimpleRecommendationsForScenario()
        {
            var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;
            _scenarioResultsContext.RecommendationsForScenarios = await _scenarioResultsApi
                .GetSimleRecommendationsForScenario(scenarioId)
                .ConfigureAwait(false);
        }

        [When(@"I request aggregated recommendations for scenario")]
        public async Task WhenIRequestAggregatedRecommendationsForScenario()
        {
            var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;
            _scenarioResultsContext.AggregatedReccomendations = await _scenarioResultsApi
                .GetAggregatedReccomendations(scenarioId)
                .ConfigureAwait(false);
        }

        [Then(@"aggregated recommendations are returned")]
        public void ThenAggregatedRecommendationsAreReturned()
        {
            Assert.IsNotEmpty(_scenarioResultsContext.AggregatedReccomendations);
        }

        [When(@"I request valid files by ids")]
        public async Task WhenIRequestValidFilesByIds()
        {
            var scenarioId = _scenarioResultsContext.RunResultData.ScenarioId;
            var outputFiles = new List<string>();

            foreach (string key in _scenarioResultsContext.RunResultData.FileNamesAndLengths.Select(x => x.Key))
            {
                var fileName = key.Replace("file:", string.Empty);
                var response = await _scenarioResultsApi
                    .GetOutputFilesByIdResponse(scenarioId, Path.GetFileNameWithoutExtension(fileName))
                    .ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    // to make sure that file content is returned
                    var fileContent = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                    outputFiles.Add(key);
                }
            }
            _scenarioResultsContext.ReturnedOutputFiles = outputFiles;
        }

        [Then(@"same as created files are returned")]
        public void ThenSameAsCreatedFilesAreReturned()
        {
            Assert.AreEqual(_scenarioResultsContext.RunResultData.FileNamesAndLengths.Select(x => x.Key),
                            _scenarioResultsContext.ReturnedOutputFiles);
        }
    }
}
