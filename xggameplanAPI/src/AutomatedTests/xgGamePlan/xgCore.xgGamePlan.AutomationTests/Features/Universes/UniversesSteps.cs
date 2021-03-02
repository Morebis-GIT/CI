using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Demographics;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Models.Universes;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Universes
{
    [Binding]
    public class UniversesSteps : BaseSteps<IUniversesApi>
    {
        public UniversesSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I know how many Universes there are")]
        public async Task GivenIKnowHowManyUniversesThereAre()
        {
            var existingUniverses = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(existingUniverses.Count(), "universesNumber");
        }

        [When(@"I add (.*) Universes")]
        public async Task WhenIAddUniverses(int count)
        {
            ScenarioContext.Set(count, "additionalUniverses");
            
            var universes = BuildUniverses();
            await CreateUniverses(universes).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"(.*) additional Universes are returned")]
        public async Task ThenAdditionalUniversesAreReturned(int additionalUniversesNumber)
        {
            var existingUniverses = await Api.GetAll().ConfigureAwait(false);
            var existingUniversesNumber = existingUniverses.Count();
            var previousUniversesNumber = ScenarioContext.Get<int>("universesNumber");

            Assert.IsTrue(existingUniversesNumber == (previousUniversesNumber + additionalUniversesNumber));
        }

        [Given(@"I have added (.*) Universes")]
        public async Task GivenIHaveAddedUniverses(int count)
        {
            ScenarioContext.Set(count, "additionalUniverses");
            var universes = BuildUniverses();                    
            await CreateUniverses(universes).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I delete all Universes")]
        public async Task WhenIDeleteAllUniverses()
        {
            await Api.DeleteAll().ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"no Universes are returned")]
        public async void ThenNoUniversesAreReturned()
        {
            var existingUniverses = await Api.GetAll().ConfigureAwait(false);
            var existingUniversesNumber = existingUniverses.Count();

            Assert.Zero(existingUniversesNumber);
        }

        private async Task CreateUniverses(List<Universe> universes)
        {
            await Api.Create(universes).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        /// <summary>
        /// Create a Universe instance type for every Demographic
        /// </summary>
        /// <param name="demographics"></param>
        /// <returns></returns>
        private List<Universe> BuildUniverses()
        {
            //valid Sales Area should be available
            var salesAreaName = ScenarioContext.Get<SalesArea>().Name;

            var demographics = ScenarioContext.Get<List<Demographic>>();

            //valid demographic should be available for each universe
            List<Universe> universes = new List<Universe>();
            foreach (var demographic in demographics)
            {
                var universe = Fixture.Build<Universe>()
                                   .With(p => p.Demographic, demographic.ExternalRef)
                                   .With(p => p.SalesArea, salesAreaName)
                                   .Without(x => x.StartDate)
                                   .Without(x => x.EndDate)
                                   .Do(x =>
                                   {
                                       x.StartDate = Fixture.Create<DateTime>();
                                       x.EndDate = x.StartDate + Fixture.Create<TimeSpan>();
                                   }).Create();
                universes.Add(universe);
            }
            return universes;
        }
    }
}
