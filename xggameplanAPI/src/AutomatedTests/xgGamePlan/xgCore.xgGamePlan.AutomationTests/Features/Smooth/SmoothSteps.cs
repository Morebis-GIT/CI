using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Smooth;
using xgCore.xgGamePlan.ApiEndPoints.Routes;

namespace xgCore.xgGamePlan.AutomationTests.Features.Smooth
{
    [Binding]
    public class SmoothSteps : BaseSteps<ISmoothApi>
    {
        private readonly ITestEnvironmentMaintenanceApi _testEnvironmentMaintenanceApi;

        public SmoothSteps(ScenarioContext scenarioContext,
            ITestEnvironmentMaintenanceApi testEnvironmentMaintenanceApi) : base(scenarioContext)
        {
            _testEnvironmentMaintenanceApi = testEnvironmentMaintenanceApi;
        }

        [Given(@"I have a valid SmoothConfiguration")]
        public async Task GivenIHaveAValidSmoothConfiguration()
        {
            var smoothConfigurationId = await _testEnvironmentMaintenanceApi
                                        .PopulateSmoothConfiguration()
                                        .ConfigureAwait(false);
            ScenarioContext.Set(smoothConfigurationId);
        }

        [When(@"I validate SmoothConfiguration by id")]
        public async Task WhenIValidateSmoothConfigurationById()
        {
            int smoothConfigurationId = ScenarioContext.Get<int>();
            var validationResult = await Api.ValidateSmoothConfiguration(smoothConfigurationId)
                                               .ConfigureAwait(false);
            ScenarioContext.Set(validationResult);
        }

        [When(@"I export SmoothConfiguration for best break factor groups")]
        public async Task WhenIExportSmoothConfigurationForBestBreakFactorGroups()
        {
            int smoothConfigurationId = ScenarioContext.Get<int>();
            await Api.ExportSmoothConfigurationForBestBreakFactorGroups(smoothConfigurationId)
                                               .ConfigureAwait(false);
        }

        [When(@"I export SmoothConfiguration for passes")]
        public async Task WhenIExportSmoothConfigurationForPasses()
        {
            int smoothConfigurationId = ScenarioContext.Get<int>();
            await Api.ExportSmoothConfigurationForPasses(smoothConfigurationId)
                                               .ConfigureAwait(false);
        }

        [Then(@"no error messages are returned")]
        public void ThenNoErrorMessagesAreReturned()
        {
            var validationResult = ScenarioContext.Get<SmoothValidationResult>();
            Assert.IsEmpty(validationResult.Messages);
        }
    }
}
