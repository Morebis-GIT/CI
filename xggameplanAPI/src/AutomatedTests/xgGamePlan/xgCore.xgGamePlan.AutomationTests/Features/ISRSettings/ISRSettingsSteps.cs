using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.ISRSettings;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.ISRSettings
{
    [Binding]
    public class ISRSettingsSteps : BaseSteps<IISRSettingsApi>
    {
        public ISRSettingsSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given("I have added ISRSettings for Sales Area")]
        public async Task GivenIHaveAddedISRSettingsForSalesArea()
        {
            var salesArea = ScenarioContext.Get<SalesArea>();
            var isrSettings = Fixture.Create<ISRSettingsModel>();
            isrSettings.SalesArea = salesArea.Name;
            var createdIsrSettings = await Api.Create(isrSettings).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            ScenarioContext.Set(createdIsrSettings);
        }

        [When(@"I get ISRSettings for Sales Area")]
        public async Task WhenIGetISRSettingsForSalesArea()
        {
            var salesArea = ScenarioContext.Get<SalesArea>();
            var isrSettings = await Api.GetISRSettings(salesArea.Name).ConfigureAwait(false);
            ScenarioContext.Set(isrSettings);
        }

        [When(@"I compare ISRSettings")]
        public async Task WhenICompareISRSettings()
        {
            var compareResult = await Api.Compare().ConfigureAwait(false);
            ScenarioContext.Set(compareResult);
        }

        [When(@"I update ISRSettings")]
        public async Task WhenIUpdateISRSettings()
        {
            var isrSettings = ScenarioContext.Get<ISRSettingsModel>();
            var updatedIsrSettings = await Api.Update(isrSettings).ConfigureAwait(false);
            ScenarioContext.Set(updatedIsrSettings);
        }

        [Then(@"updated ISRSettings is returned")]
        public void ThenUpdatedISRSettingsIsReturned()
        {
            var isrSettings = ScenarioContext.Get<ISRSettingsModel>();
            Assert.IsNotNull(isrSettings);
        }

        [Then(@"ISRSettings is returned")]
        public void ThenISRSettingsIsReturned()
        {
            var isrSettings = ScenarioContext.Get<ISRSettingsModel>();
            Assert.IsNotNull(isrSettings);
        }

        [Then(@"compare result is returned")]
        public void ThenCompareResultIsReturned()
        {
            var compareResult = ScenarioContext.Get<ISRSettingsCompareModel>();
            Assert.IsNotNull(compareResult);
        }
    }
}
