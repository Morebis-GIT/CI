using System.Threading.Tasks;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Features.FeatureFlags
{
    [Binding]
    public class FeatureFlagsSteps : BaseSteps<IFeatureFlagsApi>
    {
        public FeatureFlagsSteps(ScenarioContext context) : base(context)
        {
        }

        [When(@"I request all FeatureFlags")]
        public async Task WhenIRequestAllFeatureFlags()
        {
            await Api.GetAll().ConfigureAwait(false);
        }
    }
}
