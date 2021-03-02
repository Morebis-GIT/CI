using System;
using TechTalk.SpecFlow;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Steps.DataStorage
{
    [Binding]
    public class GeneralSteps
    {
        private readonly ITestClock _testClock;

        public GeneralSteps(ITestClock testClock)
        {
            _testClock = testClock;
        }

        [Given("the current date is '(.*)'")]
        [When("the current date is '(.*)'")]
        public void TheCurrentDateIs(DateTime value)
        {
            _testClock.Freeze(value);
        }
    }
}
