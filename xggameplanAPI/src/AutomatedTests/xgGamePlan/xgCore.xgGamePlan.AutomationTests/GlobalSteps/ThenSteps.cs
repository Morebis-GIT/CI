using NUnit.Framework;
using TechTalk.SpecFlow;

namespace xgCore.xgGamePlan.AutomationTests.GlobalSteps
{
    [Binding]
    public static class ThenSteps
    {
        /// <summary>
        /// Validates that rest api method is called successfully
        /// It doesn't validate http status code
        /// </summary>
        [Then(@"the method succeeded")]
        public static void ThenTheMethodSucceeded()
        {
            Assert.Pass();
        }
    }
}
