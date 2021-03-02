using System.Threading.Tasks;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Features.FlexibilityLevels
{
    [Binding]
    public class FlexibilityLevelsSteps : BaseSteps<IFlexibilityLevelsApi>
    {
        public FlexibilityLevelsSteps(ScenarioContext context) : base(context)
        {
        }

        [When(@"I request all FlexibilityLevels")]
        public async Task WhenIRequestAllFlexibilityLevels()
        {
            await Api.GetAll().ConfigureAwait(false);
        }
    }
}
