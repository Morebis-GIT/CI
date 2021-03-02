using System.Threading.Tasks;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Features.FunctionalAreas
{
    [Binding]
    public class FunctionalAreasSteps : BaseSteps<IFunctionalAreasApi>
    {
        public FunctionalAreasSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [When(@"I request all FunctionalAreas")]
        public async Task WhenIRequestAllFunctionalAreas()
        {
            await Api.GetAll().ConfigureAwait(false);
        }
    }
}
