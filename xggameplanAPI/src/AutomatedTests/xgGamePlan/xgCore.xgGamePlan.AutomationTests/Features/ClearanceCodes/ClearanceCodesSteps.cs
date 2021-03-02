using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.ClearanceCodes;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.ClearanceCodes
{
    [Binding]
    public class ClearanceCodeSteps : BaseSteps<IClearanceCodeApi>
    {
        public ClearanceCodeSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I have a valid Clearance Code")]
        public async Task GivenIHaveAValidClearanceCode()
        {

            var existingClearanceCodes = await Api.GetAll().ConfigureAwait(false);
            if ((existingClearanceCodes?.Count ?? 0) == 0)
            {
                await CreateClearanceCodes(1).ConfigureAwait(false);
                existingClearanceCodes = await Api.GetAll().ConfigureAwait(false);
            }
            ScenarioContext.Set(existingClearanceCodes?.FirstOrDefault());
        }

        [Given(@"I know how many Clearance Codes there are")]
        public async Task GivenIKnowHowManyClearanceCodesThereAre()
        {
            var existingClearanceCodes = await Api.GetAll().ConfigureAwait(false);
            ScenarioContext.Set(existingClearanceCodes?.Count() ?? 0, "clearanceCodesNumber");
        }

        [When(@"I add (.*) Clearance Codes")]
        public async Task WhenIAddClearanceCodes(int count)
        {
            ScenarioContext.Set(count, "additionalClearanceCodesNumber");
            await CreateClearanceCodes(count).ConfigureAwait(false);
        }

        [Then(@"(.*) additional Clearance Codes are returned")]
        public async Task ThenAdditionalClearanceCodesAreReturned(int additionalUniversesNumber)
        {
            var existingClearanceCodes = await Api.GetAll().ConfigureAwait(false);
            var existingUniversesNumber = existingClearanceCodes?.Count() ?? 0;
            var previousUniversesNumber = ScenarioContext.Get<int>("clearanceCodesNumber");

            Assert.IsTrue(existingUniversesNumber == (previousUniversesNumber + additionalUniversesNumber));
        }

        public async Task CreateClearanceCodes(int count)
        {
            var newClearanceCodes = Fixture.Build<ClearanceCode>()
                .CreateMany(count)
                .ToList();
            await Api.Create(newClearanceCodes).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }
    }
}
