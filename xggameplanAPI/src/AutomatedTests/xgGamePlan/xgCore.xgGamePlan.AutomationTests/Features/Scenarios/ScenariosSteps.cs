using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models;
using xgCore.xgGamePlan.ApiEndPoints.Models.Scenarios;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Coordinators;

namespace xgCore.xgGamePlan.AutomationTests.Features.Scenarios
{
    [Binding]
    public class ScenariosSteps : BaseSteps<IScenariosApi>
    {
        private readonly ScenariosCoordinator _scenariosCoordinator;

        public ScenariosSteps(ScenarioContext context, ScenariosCoordinator scenariosCoordinator) : base(context)
        {
            _scenariosCoordinator = scenariosCoordinator;
        }

        [Given(@"I know how many Scenarios there are")]
        public async Task GivenIKnowHowManyScenariosThereAre()
        {
            var scenarios = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(scenarios == null ? 0 : scenarios.Count());
        }

        [When(@"I add (.*) Scenarios")]
        public async Task WhenIAddScenarios(int count)
        {
            var scenarios = await _scenariosCoordinator.CreateScenariosAsync(count).ConfigureAwait(false);
            ScenarioContext.Set(scenarios);
        }

        [Then(@"(.*) additional Scenarios are returned")]
        public async Task ThenAdditionalScenariosAreReturned(int additionalCount)
        {
            int previousCount = ScenarioContext.Get<int>();
            int actualCount = (await Api.GetAll().ConfigureAwait(false)).Count();

            Assert.AreEqual(actualCount, previousCount + additionalCount);
        }

        [Given(@"I have added (.*) Scenario")]
        public async Task GivenIHaveAddedScenario(int count)
        {
            var scenarios = await _scenariosCoordinator.CreateScenariosAsync(count).ConfigureAwait(false);
            ScenarioContext.Set(scenarios);
        }

        [When(@"I request my Scenario by ID")]
        public async Task WhenIRequestMyScenarioByID()
        {
            var firstScenario = ScenarioContext.Get<IEnumerable<Scenario>>().FirstOrDefault();
            var scenario = await Api.GetById(firstScenario.Id).ConfigureAwait(false);
            ScenarioContext.Set(scenario);
        }

        [Then(@"requested Scenario with ID is returned")]
        public void ThenRequestedScenarioWithIDIsReturned()
        {
            var firstScenario = ScenarioContext.Get<IEnumerable<Scenario>>().FirstOrDefault();
            var scenario = ScenarioContext.Get<Scenario>();

            Assert.AreEqual(firstScenario.Id, scenario.Id);
        }

        [When(@"I update Scenario by ID")]
        public async Task WhenIUpdateScenarioByID()
        {
            var scenario = ScenarioContext.Get<IEnumerable<Scenario>>().FirstOrDefault();
            var newScenario = _scenariosCoordinator.BuildScenarios(1).Single();
            newScenario.Id = new Guid();
            
            var updatedScenario = await Api.UpdateById(scenario.Id, newScenario).ConfigureAwait(false);
            ScenarioContext.Set(updatedScenario);
        }

        [Then(@"updated Scenario is returned")]
        public async Task ThenUpdatedScenarioIsReturned()
        {
            var updatedScenario = ScenarioContext.Get<Scenario>();
            var scenario = await Api.GetById(updatedScenario.Id).ConfigureAwait(false);

            Assert.AreEqual(scenario.Id, updatedScenario.Id);
        }

        [When(@"I remove my Scenario by ID")]
        public async Task WhenIRemoveMyScenarioByID()
        {
            var scenario = ScenarioContext.Get<IEnumerable<Scenario>>().FirstOrDefault();
            await Api.DeleteById(scenario.Id).ConfigureAwait(false);
        }

        [Then(@"no Scenario with ID is returned")]
        public async Task ThenNoScenarioWithIDIsReturned()
        {
            var scenario = ScenarioContext.Get<IEnumerable<Scenario>>().FirstOrDefault();
            var res = await Api.GetByIdResponse(scenario.Id).ConfigureAwait(false);
            Assert.AreEqual(res.StatusCode, HttpStatusCode.NotFound);
        }

        [When(@"I set Default Scenario's ID")]
        public async Task WhenISetDefaultScenarioSID()
        {
            var scenario = ScenarioContext.Get<IEnumerable<Scenario>>().FirstOrDefault();
            await Api.UpdateDefault(scenario.Id).ConfigureAwait(false);
        }

        [Then(@"Default Scenario's ID is returned")]
        public async Task ThenDefaultScenarioSIDIsReturned()
        {
            var scenario = ScenarioContext.Get<IEnumerable<Scenario>>().FirstOrDefault();
            var defaultScenarioId = (await Api.GetDefault().ConfigureAwait(false)).ScenarioId;

            Assert.AreEqual(scenario.Id, defaultScenarioId);
        }

        [When(@"I search my Scenario by Name")]
        public async Task WhenISearchMyScenarioByName()
        {
            var scenario = ScenarioContext.Get<IEnumerable<Scenario>>().FirstOrDefault();
            var scenariosSearchResult = await Api.Search(new ScenarioSearchQueryModel { Name = scenario.Name }).ConfigureAwait(false);
            ScenarioContext.Set(scenariosSearchResult);
        }

        [Then(@"requested Scenario with Name is found")]
        public void ThenRequestedScenarioWithNameIsFound()
        {
            var scenario = ScenarioContext.Get<IEnumerable<Scenario>>().FirstOrDefault();
            var scenarioSearchResult = ScenarioContext.Get<SearchResultModel<Scenario>>();

            Assert.AreEqual(scenario.Name, scenarioSearchResult.Items.FirstOrDefault().Name);
        }
    }
}
