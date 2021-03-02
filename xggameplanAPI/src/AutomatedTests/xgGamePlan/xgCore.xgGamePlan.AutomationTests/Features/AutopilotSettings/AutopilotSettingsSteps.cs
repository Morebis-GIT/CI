using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.AutopilotSettings;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Features.AutopilotSettings
{
    [Binding]
    public class AutopilotSettingsSteps : BaseSteps<IAutopilotSettingsApi>
    {
        public AutopilotSettingsSteps(ScenarioContext context) : base(context)
        {
        }

        [When(@"I request AutopilotSettings")]
        public async Task WhenIRequestAutopilotSettings()
        {
            await Api.GetDefault().ConfigureAwait(false);
        }

        [Given("I have a valid AutopilotSettings")]
        public async Task GivenIHaveAValidAutopilotSettings()
        {
            var autopilotSettings = await Api.GetDefault().ConfigureAwait(false);
            ScenarioContext.Set(autopilotSettings, "existingModel");
        }

        [When(@"I update AutopilotSettings by Id")]
        public async Task WhenIUpdateAutopilotSettingsById()
        {
            var existingModel = ScenarioContext.Get<AutopilotSettingsModel>("existingModel");
            var newModel = BuildAutopilotSettingsFixture();
            newModel.Id = existingModel.Id;
            newModel.DefaultFlexibilityLevelId = 3;

            var updatedModel = await Api.PutDefault(newModel.Id, newModel).ConfigureAwait(false);
            ScenarioContext.Set(updatedModel, "updatedModel");
        }

        [When(@"I configure autopilot to return (.*) scenarios")]
        public async Task WhenIConfigureAutopilotToReturnScenarios(int count)
        {
            var existingModel = await Api.GetDefault().ConfigureAwait(false);
            var newModel = BuildAutopilotSettingsFixture(count);

            var updatedModel = await Api.PutDefault(existingModel.Id, newModel).ConfigureAwait(false);
            ScenarioContext.Set(count);
            ScenarioContext.Set(updatedModel, "updatedModel");
        }

        [Then(@"updated AutopilotSettings are returned")]
        public async Task ThenUpdatedAutopilotSettingsAreReturned()
        {
            var currentModel = await Api.GetDefault().ConfigureAwait(false);
            var updatedModel = ScenarioContext.Get<AutopilotSettingsModel>("updatedModel");

            Assert.That(currentModel.Id == updatedModel.Id &&
                        currentModel.DefaultFlexibilityLevelId == updatedModel.DefaultFlexibilityLevelId);
        }

        private UpdateAutopilotSettingsModel BuildAutopilotSettingsFixture(int scenariosNumber = 8)
        {
            var autopilotRules = Fixture.Build<UpdateAutopilotRuleModel>()
                .With(r => r.UniqueRuleKey, "1_1")
                .With(r => r.Enabled, true)
                .CreateMany(1);

            return Fixture.Build<UpdateAutopilotSettingsModel>()
                .With(r => r.Id, 1)
                .With(a => a.DefaultFlexibilityLevelId, 1)
                .With(a => a.ScenariosToGenerate, scenariosNumber)
                .With(a => a.AutopilotRules, autopilotRules.ToList())
                .Create();
        }
    }
}
