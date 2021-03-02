using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns;
using xgCore.xgGamePlan.ApiEndPoints.Models.Scenarios;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Coordinators
{
    [Binding]
    public class ScenariosCoordinator
    {
        private readonly IScenariosApi _scenariosApi;
        private readonly IFixture _fixture;
        private readonly PassesCoordinator _passesCoordinator;

        public ScenariosCoordinator(IScenariosApi scenariosApi, IFixture fixture, PassesCoordinator passesCoordinator)
        {
            _scenariosApi = scenariosApi;
            _fixture = fixture;
            _passesCoordinator = passesCoordinator;
        }

        public IEnumerable<Scenario> BuildScenarios(int count)
        {
            return _fixture.Build<Scenario>()
                .Without(p => p.CompletedDateTime)
                .Without(p => p.StartedDateTime)
                .Without(p => p.Progress)
                .With(p => p.IsDefault, false)
                .With(p => p.Passes, () => _passesCoordinator.BuildPasses(1, false).ToList())
                .With(p => p.CampaignPassPriorities, new List<CampaignPassPriorityModel>())
                .With(p => p.DateCreated, DateTime.UtcNow)
                .With(p => p.DateModified, DateTime.UtcNow)
                .CreateMany(count);
        }

        public async Task<IEnumerable<Scenario>> CreateScenariosAsync(int count)
        {
            var scenarioFixtures = BuildScenarios(count);
            var scenarios = await Task.WhenAll(scenarioFixtures.Select(s => _scenariosApi.Create(s)))
                .ConfigureAwait(false);
            return scenarios;
        }
    }
}
