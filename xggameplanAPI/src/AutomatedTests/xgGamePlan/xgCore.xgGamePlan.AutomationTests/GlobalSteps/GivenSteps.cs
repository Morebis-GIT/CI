using System;
using AutoFixture;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints;

namespace xgCore.xgGamePlan.AutomationTests.GlobalSteps
{
    [Binding]
    public class GivenSteps
    {
        //raven needs some time to re-index data after save, get methods can't be call immediately
        public const int delayForSave = 0;

        private readonly ConfigurationReader _configReader = new ConfigurationReader("config.json");
        private readonly ScenarioContext _scenarioContext;
        private readonly Fixture _fixture;

        public GivenSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _fixture = new Fixture();
        }

        [Given(@"I have a valid authentication token")]
        public void GivenIHaveAValidAuthenticationToken()
        {
            _scenarioContext.Set($"Bearer {_configReader.AccessToken}", "accessToken");
        }

        [Given(@"I have a valid (.*) days date range")]
        public void GivenIHaveAValidDateRange(int count)
        {
            var dateStart = _fixture.Create<DateTime>();
            var dateEnd = dateStart.AddDays(count);
            var dateRange = (dateStart, dateEnd);
            _scenarioContext.Set(dateRange);
        }
    }
}
