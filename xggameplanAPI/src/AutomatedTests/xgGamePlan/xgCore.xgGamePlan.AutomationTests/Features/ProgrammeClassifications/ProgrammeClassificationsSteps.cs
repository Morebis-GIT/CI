using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.ProgrammeClassifications;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.ProgrammeClassifications
{
    [Binding]
    public class ProgrammeClassificationsSteps : BaseSteps<IProgrammeClassificationsApi>
    {
        public ProgrammeClassificationsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I know how many ProgrammeClassifications there are")]
        public async Task GivenIKnowHowManyProgrammeClassificationsThereAre()
        {
            var classifications = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(classifications.Count());
        }

        [Given(@"I have added (.*) ProgrammeClassifications")]
        public async Task GivenIHaveAddedProgrammeClassifications(int count)
        {
            await CreateProgrammeClassification(count).ConfigureAwait(false);
        }

        [When(@"I add (.*) ProgrammeClassifications")]
        public async Task WhenIAddProgrammeClassifications(int count)
        {
            await CreateProgrammeClassification(count).ConfigureAwait(false);
        }

        [When(@"I delete all ProgrammeClassifications")]
        public async Task WhenIDeleteAllProgrammeClassifications()
        {
            await Api.DeleteAll().ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"(.*) additional ProgrammeClassifications are returned")]
        public async Task ThenAdditionalProgrammeClassificationsAreReturned(int count)
        {
            var allClassifications = await Api.GetAll().ConfigureAwait(false);

            var existingClassifications = ScenarioContext.Get<int>();
            Assert.That(allClassifications.Count(), Is.EqualTo(existingClassifications + count));
        }

        [Then(@"no ProgrammeClassifications are returned")]
        public async void ThenNoProgrammeClassificationsAreReturned()
        {
            var classifications = await Api.GetAll().ConfigureAwait(false);
            Assert.IsEmpty(classifications);
        }

        private async Task CreateProgrammeClassification(int count)
        {
            var values = Fixture.CreateMany<ProgrammeClassification>(count);
            await Api.Create(values).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }
    }
}
