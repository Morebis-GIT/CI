using System.Threading.Tasks;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Features.Languages
{
    [Binding]
    public class LanguagesSteps : BaseSteps<ILanguagesApi>
    {
        public LanguagesSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [When(@"I request all languages")]
        public async Task WhenIRequestAllLanguages()
        {
            await Api.GetAll().ConfigureAwait(false);
        }
    }
}
