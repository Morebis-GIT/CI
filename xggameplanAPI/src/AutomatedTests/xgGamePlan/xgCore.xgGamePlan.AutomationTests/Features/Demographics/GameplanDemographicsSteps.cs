using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Demographics;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Demographics
{
    [Binding]
    public class GameplanDemographicsSteps : BaseSteps<IDemographicsApi>
    {
        public GameplanDemographicsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I know how many GamePlan demographics there are")]
        public async Task GivenIKnowHowManyGamePlanDemographicsThereAre()
        {
            var existingDemographics = await Api.GetAll().ConfigureAwait(false);
            var existingGamePlanDemographics = existingDemographics.Where(x => x.Gameplan);
            ScenarioContext.Set(existingGamePlanDemographics.Count());
        }

        [When(@"I add (.*) GamePlan demographics")]
        public async Task WhenIAddGamePlanDemographics(int count)
        {
            var newGamePlanDemographics = Fixture.Build<Demographic>()
                                                  .With(p => p.Gameplan, true)
                                                  .CreateMany(count).ToList();
            await CreateDemographics(newGamePlanDemographics).ConfigureAwait(false);
        }

        [When(@"(.*) other demographics")]
        public async Task WhenIAddOtherDemographics(int count)
        {
            var newOtherDemographics = Fixture.Build<Demographic>()
                                                .With(p => p.Gameplan, false)
                                                .CreateMany(count).ToList();
            newOtherDemographics.ForEach(x => x.Gameplan = false);
            await CreateDemographics(newOtherDemographics).ConfigureAwait(false);
        }

        [Then(@"(.*) additional GamePlan demographics are returned")]
        public async Task ThenAdditionalGamePlanDemographicsAreReturned(int count)
        {
            var gamePlanDemographics = await Api.GetAllGamePlan().ConfigureAwait(false);

            var existingGamePlanDemographics = ScenarioContext.Get<int>();
            Assert.That(gamePlanDemographics.Count, Is.EqualTo(existingGamePlanDemographics + count));
        }

        [Given(@"I have (.*) valid Demographics")]
        public async Task GivenIHaveValidDemographics(int count)
        {
            Fixture _fixture = new Fixture();
            var demographics = _fixture.Build<Demographic>()
                                                  .With(p => p.Gameplan, true)
                                                  .CreateMany(count).ToList();
            await CreateDemographics(demographics).ConfigureAwait(false);
            ScenarioContext.Set(demographics);
        }

        [Given(@"I have a valid Demographic")]
        public async Task GivenIHaveAValidDemographic()
        {
            Fixture _fixture = new Fixture();
            var demographics = _fixture.Build<Demographic>()
                .Without(p => p.Id)
                .With(p => p.Gameplan, true)
                .CreateMany(1);
            await CreateDemographics(demographics).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            var validDemographic = await Api.GetByExternalRef(demographics.FirstOrDefault().ExternalRef).ConfigureAwait(false);
            ScenarioContext.Set(validDemographic);
        }

        private async Task CreateDemographics(IEnumerable<Demographic> demographics)
        {
            await Api.Create(demographics).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }
    }
}
