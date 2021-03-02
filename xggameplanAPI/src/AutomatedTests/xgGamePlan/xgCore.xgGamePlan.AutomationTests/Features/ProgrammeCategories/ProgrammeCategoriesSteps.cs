using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.ProgrammeCategory;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.ProgrammeCategories
{
    [Binding]
    public class ProgrammeCategoriesSteps : BaseSteps<IProgrammeCategoriesApi>
    {
        public ProgrammeCategoriesSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I know how many ProgrammeCategories there are")]
        public async Task GivenIKnowHowManyProgrammeCategoriesThereAre()
        {
            var programmeCategories = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(programmeCategories.Count());
        }

        [Given(@"I have added (.*) ProgrammeCategories")]
        public async Task GivenIHaveAddedProgrammeCategories(int count)
        {
            await CreateProgrammeCategories(count).ConfigureAwait(false);
        }

        [Given(@"I have a valid Programme Categories")]
        public async Task GivenIHaveAValidProgrammeCategories()
        {
            var programmeCategories = new List<ProgrammeCategoryHierarchy>
            {
                Fixture.Build<ProgrammeCategoryHierarchy>()
                    .With(p => p.Id, 1)
                    .With(p => p.Name, "SPORTS")
                    .Create(),
                Fixture.Build<ProgrammeCategoryHierarchy>()
                    .With(p => p.Id, 2)
                    .With(p => p.Name, "CHILDREN")
                    .Create(),
                Fixture.Build<ProgrammeCategoryHierarchy>()
                    .With(p => p.Id, 3)
                    .With(p => p.Name, "NEWS")
                    .Create(),
                Fixture.Build<ProgrammeCategoryHierarchy>()
                    .With(p => p.Id, 4)
                    .With(p => p.Name, "OTHER")
                    .Create()
            };

            await Api.Create(programmeCategories).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            ScenarioContext.Set(programmeCategories);
        }

        [When(@"I delete all ProgrammeCategories")]
        public async Task WhenIDeleteAllProgrammeCategories()
        {
            await Api.DeleteAll().ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I add (.*) ProgrammeCategories")]
        public async Task WhenIAddProgrammeCategories(int count)
        {
            await CreateProgrammeCategories(count).ConfigureAwait(false);
        }

        [Then(@"(.*) additional ProgrammeCategories are returned")]
        public async Task ThenAdditionalProgrammeCategoriesAreReturned(int count)
        {
            var allProgrammeCategories = await Api.GetAll().ConfigureAwait(false);

            int existingProgrammeCategories = ScenarioContext.Get<int>();
            Assert.That(allProgrammeCategories.Count(), Is.EqualTo(existingProgrammeCategories + count));
        }

        [Then(@"no ProgrammeCategories are returned")]
        public async void ThenNoProgrammeCategoriesAreReturned()
        {
            var programmeCategories = await Api.GetAll().ConfigureAwait(false);
            Assert.IsEmpty(programmeCategories);
        }

        private async Task CreateProgrammeCategories(int count)
        {
            var values = Fixture.CreateMany<ProgrammeCategoryHierarchy>(count).ToList();
            await Api.Create(values).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }
    }
}
