using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.ApiConnectivity;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Features;

namespace xgCore.xgGamePlan.AutomationTests.Steps
{
    [Binding]
    public class APIConnectivitySteps : BaseSteps<IApiConnectivity>
    {
        public APIConnectivitySteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I have a URL for an xgGamePlan API")]
        public void GivenIHaveAURLForAnXgGamePlanAPI()
        {
            ScenarioContext.Set(Api);
        }

        [Then(@"the correct version is returned")]
        public void ThenTheCorrectVersionIsReturned()
        {
            var _apiVersionResult = ScenarioContext.Get<ApiVersionResult>();

            Assert.That(_apiVersionResult.Version, Is.EqualTo(ConfigReader.ApiVersion));
        }

        [When(@"I query the API test ""(.*)"" Verb")]
        public async Task WhenIQueryTheAPITestVerb(string httpVerb)
        {
            var api = ScenarioContext.Get<IApiConnectivity>();

            Func<Task<TestVerbResult>> verbToTest;

            switch (httpVerb.ToUpperInvariant())
            {
                case "GET":
                    verbToTest = api.TestGetVerb;
                    break;

                case "POST":
                    verbToTest = api.TestPostVerb;
                    break;

                case "PUT":
                    verbToTest = api.TestPutVerb;
                    break;

                case "DELETE":
                    verbToTest = api.TestDeleteVerb;
                    break;

                case "PATCH":
                    verbToTest = api.TestPatchVerb;
                    break;

                default:
                    throw new ArgumentException(
                        $"The HTTP verb {httpVerb} is not a valid test case.",
                        nameof(httpVerb)
                    );
            }

            await verbToTest()
                .ConfigureAwait(false);
        }
    }
}
