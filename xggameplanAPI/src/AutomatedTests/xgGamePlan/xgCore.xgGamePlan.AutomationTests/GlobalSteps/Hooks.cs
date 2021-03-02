using Refit;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using xgCore.xgGamePlan.ApiEndPoints.Api;
using xgCore.xgGamePlan.AutomationTests.Extensions;

namespace xgCore.xgGamePlan.AutomationTests.GlobalSteps
{
    [Binding]
    public static class Hooks
    {
        [AfterStep]
        public static void AfterScenarioStep(ScenarioContext scenarioContext)
        {
            if (scenarioContext.TestError is ApiException apiException)
            {
                scenarioContext.SetMemberValue(x => x.TestError, new ServerResponseException(apiException));
            }
        }
    }
}
