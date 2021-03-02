using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.Runs;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Coordinators
{
    [Binding]
    public class RunsCoordinator
    {
        private readonly IRunsApi _runsApi;
        private readonly IFixture _fixture;
        private readonly ScenariosCoordinator _scenariosCoordinator;

        public RunsCoordinator(IRunsApi runsApi, IFixture fixture, ScenariosCoordinator scenariosCoordinator)
        {
            _runsApi = runsApi;
            _fixture = fixture;
            _scenariosCoordinator = scenariosCoordinator;
        }

        public IEnumerable<CreateRunModel> BuildCreateRun(int count)
        {
            var runDateRange = new DateRange(DateTime.UtcNow, DateTime.UtcNow.AddDays(3));
            var smoothDateRange = new DateRange(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(2));

            return _fixture.Build<CreateRunModel>()
                .Without(p => p.AddDefaultScenario)
                .Without(p => p.ISR)
                .Without(p => p.Optimisation)
                .Without(p => p.RightSizer)
                .With(p => p.Description, "test description " + new Random().Next())
                .With(p => p.Smooth, true)
                .With(p => p.SmoothDateRange, smoothDateRange)
                .With(p => p.StartDate, runDateRange.Start)
                .With(p => p.EndDate, runDateRange.End)
                .With(p => p.IsLocked, false)
                .With(p => p.Scenarios, () => _scenariosCoordinator.BuildScenarios(1).ToList())
                .CreateMany(count);
        }

        public async Task<IEnumerable<Run>> CreateRunsAsync(int count)
        {
            var createRunFixtures = BuildCreateRun(count);
            return await Task.WhenAll(createRunFixtures.Select(s => _runsApi.Create(s))).ConfigureAwait(false);
        }
    }
}
