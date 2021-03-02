using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using NUnit.Framework;
using TechTalk.SpecFlow;
using xgCore.xgGamePlan.ApiEndPoints.Models.Breaks;
using xgCore.xgGamePlan.ApiEndPoints.Models.SalesAreas;
using xgCore.xgGamePlan.ApiEndPoints.Routes;
using xgCore.xgGamePlan.AutomationTests.Extensions;
using xgCore.xgGamePlan.AutomationTests.GlobalSteps;

namespace xgCore.xgGamePlan.AutomationTests.Features.Breaks
{
    [Binding]
    public class BreaksSteps : BaseSteps<IBreaksApi>
    { 
        public BreaksSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"I know how many Breaks there are")]
        public async Task GivenIKnowHowManyBreaksThereAre()
        {
            var existingBreaks = await Api.GetAll().ConfigureAwait(false) ?? new List<Break>();
            ScenarioContext.Set(existingBreaks.Count, "breaksCount");
        }

        [Given(@"I know how many Breaks within date range there are")]
        public async Task GivenIKnowHowManyBreaksWithinDateRangeThereAre()
        {
            var existingBreaks = await GetBreaksForDateRange().ConfigureAwait(false);
            ScenarioContext.Set(existingBreaks.Count(), "breaksCount");
        }

        [When(@"I add (.*) Breaks")]
        public async Task WhenIAddBreaks(int count)
        {
            ScenarioContext.Set(count, "additionalBreaks");
            await CreateBreaks(count).ConfigureAwait(false);
        }

        [Then(@"(.*) additional Breaks are returned")]
        public async Task ThenAdditionalBreaksAreReturned(int additionalBreaksNumber)
        {
            //additional delay before retrieving breaks count because of raven data re-index
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
            var existingBreaks = await Api.GetAll().ConfigureAwait(false);
            var existingBreaksNumber = existingBreaks.Count();
            var previousBreaksNumber = ScenarioContext.Get<int>("breaksCount");

            Assert.IsTrue(existingBreaksNumber == (previousBreaksNumber + additionalBreaksNumber));
        }

        [Given(@"I have added (.*) Breaks")]
        public async Task GivenIHaveAddedBreaks(int count)
        {
            await WhenIAddBreaks(count).ConfigureAwait(false);
        }

        [When(@"I add (.*) Breaks for date range")]
        public async Task WhenIAddBreaksForDateRange(int count)
        {
            var dateRange = ScenarioContext.Get<(DateTime from, DateTime to)>();
            await CreateBreaks(count, dateRange.GetDateInRange()).ConfigureAwait(false);
        }

        [When(@"I delete Breaks by date range")]
        public async Task WhenIDeleteBreaksByDateRange()
        {
            var salesAreaName = ScenarioContext.Get<SalesArea>().Name;
            var (from, to) = ScenarioContext.Get<(DateTime from, DateTime to)>();
            await Api.Delete(from, to, new List<string> {salesAreaName}).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [When(@"I delete all Breaks")]
        public async Task WhenIDeleteAllBreaks()
        {
            await Api.DeleteAll().ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }

        [Then(@"no Breaks are returned")]
        public async Task ThenNoBreaksAreReturned()
        {
            var existingBreaks = await Api.GetAll().ConfigureAwait(false);

            Assert.IsNull(existingBreaks);
        }

        [Then(@"no Breaks within date range are returned")]
        public async Task ThenNoBreaksWithinDateRangeAreReturned()
        {
            var existingBreaks = await GetBreaksForDateRange().ConfigureAwait(false);
            Assert.IsEmpty(existingBreaks);
        }

        private async Task<IEnumerable<Break>> GetBreaksForDateRange()
        {
            var salesAreaName = ScenarioContext.Get<SalesArea>().Name;
            var (from, to) = ScenarioContext.Get<(DateTime from, DateTime to)>();
            var existingBreaks = await Api.GetAll().ConfigureAwait(false) ?? new List<Break>();

            return existingBreaks.Where(b =>
                b.SalesArea == salesAreaName && b.ScheduledDate >= from && b.ScheduledDate <= to);
        }

        private async Task CreateBreaks(int count, DateTime? date = null)
        {
            if (date == null)
            {
                date = Fixture.Create<DateTime>();
            }

            var salesAreaName = ScenarioContext.Get<SalesArea>().Name;
            var breakType = ScenarioContext.Get<List<string>>("BreakTypes").FirstOrDefault();
            var breaks = Fixture.Build<Break>()
                                   .With(p => p.BreakType, breakType)
                                   .With(p => p.SalesArea, salesAreaName)
                                   .With(p => p.ScheduledDate, date)
                                   .CreateMany(count);
            await Api.Create(breaks).ConfigureAwait(false);
            await Task.Delay(GivenSteps.delayForSave).ConfigureAwait(false);
        }
    }
}
